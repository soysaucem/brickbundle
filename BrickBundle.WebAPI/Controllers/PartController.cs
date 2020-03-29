using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrickBundle.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PartController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(DTO.ListPartsCategoriesColorsDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get()
        {
            return Ok(await Model.Part.ListPartsCategoriesColors());
        }

        [Authorize]
        [HttpPost("image")]
        [ProducesResponseType(typeof(DTO.UserPartDTO[]), StatusCodes.Status200OK)]
        public async Task<ActionResult> PostPartsImage()
        {
            if (HttpContext.Request.ContentType != "image/jpeg")
            {
                return new StatusCodeResult(StatusCodes.Status415UnsupportedMediaType);
            }
            var imageStream = HttpContext.Request.Body;
            var predictions = ML.ObjectDetection.GetInstance().GetPredictionResults(imageStream);
            // TODO: Get colors from prediction
            var partDTOs = await Model.Part.ListPartsByCode(predictions.Select(a => a.Class).Distinct());
            var userParts = new List<DTO.UserPartDTO>();
            foreach (var prediction in predictions)
            {
                var partDTO = partDTOs.First(a => a.Code == prediction.Class);
                var userPartDTO = userParts.FirstOrDefault(a => a.Part.Code == prediction.Class);
                if (userPartDTO == null)
                {
                    userParts.Add(new DTO.UserPartDTO()
                    {
                        Part = partDTO,
                        ColorQuantities = new DTO.ColorQuantityDTO[]
                        {
                            new DTO.ColorQuantityDTO()
                            {
                                Color = new DTO.ColorListItemDTO()
                                {
                                    ID = 1,
                                    Name = "[Unknown]"
                                },
                                Quantity = 1
                            }
                        }
                    });
                }
                else
                {
                    userPartDTO.ColorQuantities[0].Quantity += 1;
                }
            }
            return Ok(userParts.ToArray());
        }
    }
}
