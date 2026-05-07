using Elearning.Application.Interfaces;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Contracts.Portal.Dtos.KyThi;
using Elearning.Shared.Contracts.Portal.Querys.KyThi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elearning.API.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BaiLamController : ControllerBase
    {
        private readonly IBaiLamService _service;

        public BaiLamController(IBaiLamService service)
        {
            _service = service;
        }

        [HttpPost("getpaged-admin")]
        public async Task<ActionResult<DataTableJson>> GetPagedAdmin([FromBody] BaiLamQuery query)
        {
            try
            {
                var result = await _service.GetPagedAdminAsync(query);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/BaiLam/{baiLamId}/review
        [HttpGet("{baiLamId:guid}/review")]
        public async Task<ActionResult<BaiLamReviewDto>> GetReviewBaiLam(Guid baiLamId)
        {
            try
            {
                var result = await _service.GetChiTietBaiLamAsync(baiLamId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
