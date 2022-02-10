using Api.Pagination;
using Api.Tools.EmailHandler;
using Api.Tools.EmailHandler.Abstraction;
using AutoMapper;
using Domain.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.DAL;
using Repository.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IRepository<Student> _repository;
        private readonly IMapper _mapper;
        private readonly IEmailService _mailService;

        public StudentsController(IRepository<Student> repository, IMapper mapper, IEmailService mailService)
        {
            _mapper = mapper;
            _repository = repository;
            _mailService = mailService;
        }

        [HttpGet]

        public async Task<IActionResult> Get(int pageNumber,int itemCount)
        {
            var students = _repository.GetAllAsyncAsNoTracking();

            PaginationDto<Student> pagination = new PaginationDto<Student>(students, pageNumber, itemCount); 
          
            return Ok(pagination);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var student = await _repository.GetAsync(id);
            if (student == null) return NotFound("Bu id-de student yoxdur");
                return Ok(student);
        }

        [HttpPost]

        public async Task<IActionResult> Create([FromBody] StudentDto studentDto)
        {
            var student = _mapper.Map<Student>(studentDto);
            var result = await _repository.AddAsync(student);
            await _mailService.SendEmailAsync(new MailRequest { ToEmail = "iftikharbg@code.edu.az", Subject = "Tebrikler", Body = "Student created successfully" });
            if (!result) return BadRequest("Something bad happened");
            return StatusCode(StatusCodes.Status201Created);
            

            
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] StudentDto studentDto)
        {
            Student existStudent = await _repository.GetAsync(id);
            if (existStudent == null) return NotFound("There is no student with this id");
            existStudent.Name = studentDto.Name ;
            existStudent.Surname = studentDto.Surname;
           
            bool result = _repository.Update(existStudent);
            if (!result) return BadRequest("Something bad happened");
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var student = await _repository.GetAsync(id);
            if (!await _repository.DeleteAsync(student)) return BadRequest("Something bad happened");
            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
