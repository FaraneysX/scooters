﻿using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using ScooterRental.BL;
using ScooterRental.BL.Users.Entities;
using ScooterRental.Service.Controllers.Entities.Users;

namespace ScooterRental.Service.Controllers;

public class UsersController(IProvider<UserModel, UsersModelFilter> provider, IManager<UserModel, CreateUserModel> manager, IMapper mapper, ILogger logger) : ControllerBase
{
    private readonly IProvider<UserModel, UsersModelFilter> _provider = provider;
    private readonly IManager<UserModel, CreateUserModel> _manager = manager;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger _logger = logger;

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _provider.Get();

        return Ok(new UsersListResponse()
        {
            Users = users.ToList()
        });
    }

    [HttpGet]
    [Route("users/filter")]
    public IActionResult GetFilteredUsers([FromQuery] UsersFilter filter)
    {
        var users = _provider.Get(_mapper.Map<UsersModelFilter>(filter));

        return Ok(new UsersListResponse()
        {
            Users = users.ToList()
        });
    }

    [HttpGet("user/{id}")]
    public IActionResult GetUserInfo([FromRoute] Guid id)
    {
        try
        {
            var user = _provider.GetInfo(id);

            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex.ToString());

            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = _manager.Create(_mapper.Map<CreateUserModel>(request));

            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex.ToString());

            return BadRequest(ex.Message);
        }
    }

    [HttpPut("user/update/{id}")]
    public IActionResult UpdateUserInfo([FromRoute] Guid id, UpdateUserRequest request)
    {
        try
        {
            var user = _provider.GetInfo(id);

            if (user is null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            _mapper.Map(request, user);

            var updatedUser = _manager.Update(id, user);

            return Ok(updatedUser);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex.ToString());

            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("user/delete/{id}")]
    public IActionResult DeleteUser([FromRoute] Guid id)
    {
        try
        {
            var user = _provider.GetInfo(id);

            if (user is null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            _manager.Delete(id);

            return Ok($"User with ID {id} deleted successfully.");
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex.ToString());

            return BadRequest(ex.Message);
        }
    }
}
