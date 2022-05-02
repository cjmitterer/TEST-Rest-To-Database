using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWARM.EF.Data;
using SWARM.EF.Models;
using SWARM.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.DataSource.Extensions;

namespace SWARM.Server.Controllers.Application
{

    [Route("api/[controller]")]
    [ApiController]
    public class ZipcodeController : SuperClassController
    {

        public ZipcodeController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {

        }

        [HttpGet]
        [Route("GetZipcodes")]
        public async Task<IActionResult> GetZipcodes(){
            List<Zipcode> lst = await _context.Zipcodes.OrderBy(x => x.Zip).ToListAsync();
            return Ok(lst);
        }

        [HttpGet]
        [Route("GetZipcodes/{pZip}")]
        public async Task<IActionResult> GetZipcodes(string pZip){
            Zipcode itm = await _context.Zipcodes.Where(x => x.Zip == pZip).FirstOrDefaultAsync();
            return Ok(itm);
        }

        [HttpDelete]
        [Route("DeleteZipcode/{pZip}")]
        public async Task<IActionResult> DeleteZipcode(string pZip){
            return await base.DeleteZipcode(pZip);
        }

        [HttpPost]
        [Route("PostZipcode")]
        public async Task<IActionResult> PostZipcode([FromBody] ZipcodeDTO zipcodeDTO){
            return await ModifyOrInsertZipcode(zipcodeDTO, null);
        }

        [HttpPut]
        [Route("PutZipcode")]
        public async Task<IActionResult> PutZipcode([FromBody] ZipcodeDTO zipcodeDTO){
            return await ModifyOrInsertZipcode(zipcodeDTO, zipcodeDTO);
        }

        private async Task<IActionResult> ModifyOrInsertZipcode(ZipcodeDTO zipcodeDTO, ZipcodeDTO zipCodeDTOOld)
        {
            if (zipCodeDTOOld == null){
                zipCodeDTOOld = new ZipcodeDTO();
            }
            var trans = _context.Database.BeginTransaction();
            try
            {

                Zipcode existZipcode = await _context.Zipcodes.Where(x => x.Zip == zipCodeDTOOld.Zip).FirstOrDefaultAsync();

                bool newC = false;

                if (existZipcode == null){
                    existZipcode = new Zipcode();
                    newC = true;
                }

                existZipcode.Zip = zipcodeDTO.Zip;
                existZipcode.State = zipcodeDTO.State;
                existZipcode.City = zipcodeDTO.City;
                existZipcode.CreatedBy = zipcodeDTO.CreatedBy;
                existZipcode.CreatedDate = zipcodeDTO.CreatedDate;
                existZipcode.ModifiedBy = zipcodeDTO.ModifiedBy;
                existZipcode.ModifiedDate = zipcodeDTO.ModifiedDate;

                if (newC){
                    _context.Zipcodes.Add(existZipcode);
                }
                else{
                    _context.Update(existZipcode);
                }

                await _context.SaveChangesAsync();
                trans.Commit();



                return Ok(zipcodeDTO.Zip);

            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}