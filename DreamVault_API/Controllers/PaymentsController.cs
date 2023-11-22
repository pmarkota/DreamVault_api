using DreamVault_API.Models;
using DreamVault_API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace DreamVault_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly PostgresContext _db;
        private readonly ISubscriptionRepository _subscriberRepository;

        public PaymentsController(IConfiguration config, PostgresContext db, ISubscriptionRepository subscriberRepository)
        {
            _config = config;
            _db = db;
            _subscriberRepository = subscriberRepository;
            StripeConfiguration.ApiKey = _config.GetValue<string>("Stripe:SecretKey");
        }



        [HttpPost("create-checkout-session")]
        public async Task<ActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest req)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = _config.GetValue<string>("Payment:SuccessUrl"),
                CancelUrl = _config.GetValue<string>("Payment:CancelUrl"),
                PaymentMethodTypes = new List<string>{
                    "card",
                },
                Mode = "subscription",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = req.PriceId,
                        Quantity = 1,
                    },
                },
            };

            var service = new SessionService();
            try
            {
                var session = await service.CreateAsync(options);
                return Ok(new CreateCheckoutSessionResponse
                {
                    SessionId = session.Id
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest(new
                {
                    Message = e.Message
                });
            }
        }

        [Authorize]
        [HttpPost("customer-portal")]
        public async Task<ActionResult> CustomerPortal([FromBody] CustomerPortalRequest returnUrl)
        {
            ClaimsPrincipal principal = HttpContext.User as ClaimsPrincipal;
            var claim = principal.Claims.FirstOrDefault(c => c.Type == "username");
            var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Username == claim.Value);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid token" });
            }

            try
            {
                var options = new SessionCreateOptions
                {
                    Customer = user.CustomerId,
                    ReturnUrl = returnUrl.ReturnUrl,
                };
                var service = new SessionService();
                var session = await service.CreateAsync(options);

                return Ok(new
                {
                    url = session.Url
                });
            }
            catch (StripeException e)
            {
                Console.WriteLine(e.StripeError.Message);
                return BadRequest(new
                {
                    Message = e.StripeError.Message
                });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            string endpointSecret = _config.GetValue<string>("Stripe:WHSecret");

            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                var signatureHeader = Request.Headers["Stripe-Signature"];

                stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, endpointSecret);

                // Handle the event
                if (stripeEvent.Type == Events.CustomerCreated)
                {
                    var customer = stripeEvent.Data.Object as Customer;
                    // Do Stuff

                    // Associate the CustomerId with the user in the database
                    await addCustomerIdToUser(customer);

                    // Update the records created for customer.subscription.created event
                    await updateSubscriptionForUser(customer);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;
                    // Do stuff
                    await addSubscriptionToDb(subscription);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionUpdated)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;

                    // Update Subsription
                    await updateSubscription(subscription);
                }

                // ... handle other event types
                else
                {
                    // Unexpected event type
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return BadRequest();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return StatusCode(500);
            }
        }

        private async Task updateSubscriptionForUser(Customer customer)
        {
            try
            {
                // Find the users in the database created for customer.subscription.created
                var usersToUpdate = _db.AppUsers
                    .Where(u => u.Email == customer.Email && u.CustomerId == null)
                    .ToList();

                foreach (var user in usersToUpdate)
                {
                    // Update the user records with the associated CustomerId
                    user.CustomerId = customer.Id;
                    _db.Update(user);
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
        }



        private async Task addCustomerIdToUser(Customer customer)
        {
            try
            {
                // Try to find the user based on the email
                var userFromDb = _db.AppUsers.FirstOrDefault(u => u.Email == customer.Email);

                if (userFromDb != null)
                {
                    // If the user exists, update the CustomerId
                    userFromDb.CustomerId = customer.Id;
                    _db.Update(userFromDb);
                    await _db.SaveChangesAsync();
                    await Console.Out.WriteLineAsync("Customer ID added to user");
                }
                else
                {
                    // If the user doesn't exist, create a new user with the CustomerId
                    var newUser = new AppUser
                    {
                        Email = customer.Email,
                        CustomerId = customer.Id,
                        // Other properties...
                    };

                    _db.AppUsers.Add(newUser);
                    await _db.SaveChangesAsync();
                    await Console.Out.WriteLineAsync("New user created with Customer ID");
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
        }


        private async Task addSubscriptionToDb(Subscription subscription)
        {
            try
            {
                var subscriber = new Subscriber
                {
                    Id = subscription.Id,
                    CustomerId = subscription.CustomerId,
                    Status = subscription.Status,
                    CurrentPeriodEnd = subscription.CurrentPeriodEnd.ToString(),
                };
                var user = _db.AppUsers.FirstOrDefault(u => u.CustomerId == subscription.CustomerId);
                user.SubscriptionExpirationDate = subscription.CurrentPeriodEnd;
                _db.AppUsers.Update(user);
                await _db.SaveChangesAsync();
                await _subscriberRepository.CreateAsync(subscriber);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }
        private async Task updateSubscription(Subscription subscription)
        {
            try
            {
                var subscriptionFromDb = await _subscriberRepository.GetByIdAsync(subscription.Id);

                if (subscriptionFromDb != null)
                {
                    subscriptionFromDb.Status = subscription.Status;
                    subscriptionFromDb.CurrentPeriodEnd = subscription.CurrentPeriodEnd.ToString();
                    var user = _db.AppUsers.FirstOrDefault(u => u.CustomerId == subscription.CustomerId);
                    user.SubscriptionExpirationDate = subscription.CurrentPeriodEnd;
                    _db.AppUsers.Update(user);
                    await _db.SaveChangesAsync();
                    await _subscriberRepository.UpdateAsync(subscriptionFromDb);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }

    public class CustomerPortalRequest
    {
        [Required]
        public string ReturnUrl { get; set; }
    }

    internal class CreateCheckoutSessionResponse
    {
        public string SessionId { get; set; }
    }

    public class CreateCheckoutSessionRequest
    {
        [Required]
        public string PriceId { get; set; }
    }
}
