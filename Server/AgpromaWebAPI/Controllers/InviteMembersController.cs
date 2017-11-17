using AgProMa.Services;
using AgpromaWebAPI.model;
using AgpromaWebAPI.Service;
using AgpromaWebAPI.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ForgetPassword.Controllers
{

    [Produces("application/json")]
    public class InviteMembersController : Controller
    {

        private IinviteMembersService _service;
        private ISignUpService _context;
        public InviteMembersController(IinviteMembersService service, ISignUpService context)
        {
            _service = service;
            _context = context;
        }

        //get all details of all users
        [Route("api/[controller]")]
        public IActionResult Get()
        {
            //exception handling
            try
            {
                List<User> list = _context.GetAllDetails();
                if (list.Count != 0)
                {
                    return Ok(list);
                }
                else
                {
                    return StatusCode(404);
                }

            }
            catch
            {

                return StatusCode(500);
            }
        }


        // PUT: api/ForgetPassword/5
        [HttpPost]
        [Route("api/[controller]")]
        public IActionResult post([FromBody]InvitePeople people)
        {
            try
            {
                
                int id = _service.EmailForInvitation(people); //mail for invite people
                if(id==0)
                {
                    return Ok("Already Exist");
                }
                else
                {
                    return Ok("Mail Sent");
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.StackTrace);//handling bad request
                return BadRequest();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500); //internal server error
            }
        }

        //this method is to fetch the data
       [HttpGet]
       [Route("api/InviteMembers/{id}")]
       public List<InviteExistingMember> GetMemberName(int id)
        {
            return _service.GetMemberName(id);
        }

    }
}
