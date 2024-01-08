using DreamVault_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DreamVault_API.Controllers
{

    public class ReturnDreams
    {
        public long Id { get; set; }
        public string DreamTitle { get; set; }
        public string DreamDescription { get; set; }
        public double? DreamRating { get; set; }
        public bool? DreamPublic { get; set; }
        public DateTime DreamDate { get; set; }
        public List<string> Images { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class DreamController : ControllerBase
    {
        private PostgresContext _db;
        private IConfiguration _config;

        public DreamController(PostgresContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        //httpost to get all dreams for an appuser by token, in token there is a custom claim "id" which is the appuser id, so jwt token will be a parameter for this method
        [HttpPost("getalldreams")]
        public ActionResult<Dream> GetDreams([FromBody] GetByToken token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("Jwt:Key"));
            tokenHandler.ValidateToken(token.Token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false,
                ValidateAudience = false,
                ValidateIssuer = false
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
            var user = _db.AppUsers.FirstOrDefault(u => u.AppUserId == userId);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid token" });
            }

            var dreams = _db.Dreams.Where(d => d.AppUserId == userId).ToList();

            var returnDreams = new List<ReturnDreams>();
            foreach (var dream in dreams)
            {
                var returnDream = new ReturnDreams();
                var dreamImages = _db.DreamImages.Where(di => di.DreamId == dream.Id);
                var allDreamImages = new List<string>();
                foreach (var dreamImage in dreamImages)
                {
                    allDreamImages.Add(dreamImage.ImageUrl);
                }
                returnDream.Id = dream.Id;
                returnDream.DreamDate = dream.DreamDate;
                returnDream.DreamTitle = dream.DreamTitle;
                returnDream.Images = allDreamImages;
                returnDream.DreamDescription = dream.DreamDescription;
                returnDream.DreamRating = dream.DreamRating;
                returnDream.DreamPublic = dream.DreamPublic;

                returnDreams.Add(returnDream);
            }

            return Ok(returnDreams);
        }
        //getlatest dream for a user by token
        [Authorize]
        [HttpGet("getlatestdream")]
        public async Task<ActionResult<Dream>> GetLatestDream()
        {

            ClaimsPrincipal principal = HttpContext.User as ClaimsPrincipal;
            var claim = principal.Claims.FirstOrDefault(c => c.Type == "username");
            var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Username == claim.Value);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid token" });
            }

            var dream = await _db.Dreams.Where(d => d.AppUserId == user.AppUserId).OrderByDescending(d => d.Id).FirstOrDefaultAsync();

            if (dream == null)
            {
                return BadRequest(new { message = "No dreams found" });
            }

            var returnDream = new ReturnDreams();
            var dreamImages = _db.DreamImages.Where(di => di.DreamId == dream.Id);
            var allDreamImages = new List<string>();
            foreach (var dreamImage in dreamImages)
            {
                allDreamImages.Add(dreamImage.ImageUrl);
            }
            returnDream.Id = dream.Id;
            returnDream.DreamDate = dream.DreamDate;
            returnDream.DreamTitle = dream.DreamTitle;
            returnDream.Images = allDreamImages;
            returnDream.DreamDescription = dream.DreamDescription;
            returnDream.DreamRating = dream.DreamRating;
            returnDream.DreamPublic = dream.DreamPublic;

            return Ok(returnDream);
        }

        [HttpPost("createdream")]
        public async Task<ActionResult<Dream>> CreateDream([FromBody] DreamPostRequest req)
        {
            var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Username == req.Username);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid user" });
            }

            if (!DateTime.TryParse(req.DreamDate, out var dreamDate))
            {
                return BadRequest(new { message = "Invalid date format" });
            }

            if (dreamDate.Kind != DateTimeKind.Utc)
            {
                // Convert the DateTime to Utc if it's not already
                dreamDate = dreamDate.ToUniversalTime();
            }
            string result = null; // Declare the result variable here

            if (user.SubscriptionPlanId == 1 && user.AiImagesGeneratedThisMonth < 5)
            {
                if (!string.IsNullOrEmpty(req.DreamVisualDescription))
                {
                    // Make the API call to OpenAI here
                    result = await GenerateImage(req.DreamVisualDescription);
                    user.AiImagesGeneratedThisMonth++;
                    _db.Update(user);
                    await _db.SaveChangesAsync();
                    // You can process the result from OpenAI here
                }
            }
            else if (user.SubscriptionPlanId == 2 && user.AiImagesGeneratedThisMonth < 35)
            {
                if (!string.IsNullOrEmpty(req.DreamVisualDescription))
                {
                    // Make the API call to OpenAI here
                    result = await GenerateImage(req.DreamVisualDescription);
                    user.AiImagesGeneratedThisMonth++;
                    _db.Update(user);
                    await _db.SaveChangesAsync();
                    // You can process the result from OpenAI here
                }
            }

            var dream = new Dream
            {
                AppUserId = user.AppUserId,
                DreamTitle = req.DreamTitle,
                DreamDescription = req.DreamDescription,
                DreamDate = dreamDate,
                DreamPublic = req.DreamPublic,
                DreamVisualDescription = req.DreamVisualDescription,
                DreamRating = null
            };

            await _db.Dreams.AddAsync(dream);
            await _db.SaveChangesAsync();

            var dreamImage = new DreamImage
            {
                DreamId = dream.Id,
                ImageUrl = result
            };
            _db.DreamImages.Add(dreamImage);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Dream created successfully" });
        }

        private async Task<string> GenerateImage(string visualDescription)
        {
            using (var client = new HttpClient())
            {
                // Set the API endpoint URL and headers
                var apiUrl = "https://api.openai.com/v1/images/generations";
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.GetValue<string>("OpenAI:SecretKey")}");

                // Define and send the request payload
                var requestPayload = new
                {
                    prompt = visualDescription,
                    model = "dall-e-3",
                    n = 1,
                    size = "1024x1024",
                };

                var jsonRequest = JsonConvert.SerializeObject(requestPayload);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    var imageUrl = (string)jsonResponse.data[0].url;

                    // Use a separate HttpClient to download the image
                    byte[] imageBytes;
                    using (var imageClient = new HttpClient())
                    {
                        try
                        {
                            imageBytes = await imageClient.GetByteArrayAsync(imageUrl);
                        }
                        catch (HttpRequestException ex)
                        {
                            // Log detailed error information
                            throw new Exception($"Failed to download image: {ex.Message}", ex);
                        }
                    }

                    var fileName = $"{Guid.NewGuid()}.png";
                    var supabaseUrl = await UploadToSupabaseBucket(fileName, imageBytes);

                    return supabaseUrl;
                }
                else
                {
                    return ($"API request failed with status code {response.StatusCode}");

                }
            }
        }

        private async Task<string> UploadToSupabaseBucket(string fileName, byte[] imageBytes)
        {
            // Initialize the Supabase client
            var client = new Supabase.Client("https://bjzqexdidzsoyqgpyikb.supabase.co", _config.GetValue<string>("Supabase:Key"));

            // Save the byte array to a temporary file
            var tempFilePath = Path.GetTempFileName();
            await System.IO.File.WriteAllBytesAsync(tempFilePath, imageBytes); // Use full namespace for clarity

            // Upload the image to the bucket
            // Assuming the Upload method requires a path (string) and file name (string)
            await client.Storage.From("ai-images").Upload(tempFilePath, fileName);

            // Delete the temporary file
            System.IO.File.Delete(tempFilePath);

            // Construct the URL of the image in the Supabase bucket
            var supabaseUrl = $"https://bjzqexdidzsoyqgpyikb.supabase.co/storage/v1/object/public/ai-images/{fileName}";

            return supabaseUrl;
        }



    }

    public class DreamPostRequest
    {
        public string? Username { get; set; }
        public string? DreamTitle { get; set; }
        public string? DreamDescription { get; set; }
        public string? DreamVisualDescription { get; set; }
        public string? DreamDate { get; set; }
        public bool? DreamPublic { get; set; }
    }
}
