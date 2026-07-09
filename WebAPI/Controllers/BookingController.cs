using Microsoft.AspNetCore.Mvc;
using AIBookingSystem.DTO;
using AIBookingSystem.Services;
using AIBookingSystem.Models;

namespace AIBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<BookingDTO>> ListBookings(int userId)
    {
        if (userId <= 0)
        {
            return BadRequest("User Id is invalid.");
        }
        var bookings = _bookingService.ListBookings(userId);
        if (bookings == null || bookings.Count() == 0)
        {
            string message = "No booking is found. Please check if the user id is valid.";
            _logger.LogError(message);
            return NotFound(message);
        }

        return Ok(bookings);
    }

    [HttpPost]
    public ActionResult<BookingDTO> BookRoom([FromBody] BookingCreateDTO bookingCreateDTO)
    {
        if (bookingCreateDTO == null)
        {
            return BadRequest("Booking details are not provided.");
        }
        if (bookingCreateDTO.BookedBy == null || bookingCreateDTO.BookedBy == "")
        {
            return BadRequest("Name of the user for the booking is not provided.");
        }
        if (bookingCreateDTO.UserId <=0)
        {
            return BadRequest("Invalid user Id is provided.");
        }
        if (bookingCreateDTO.RoomId <= 0)
        {
            return BadRequest("Invalid room Id is provided.");
        }
        if (bookingCreateDTO.BookingTo < bookingCreateDTO.BookingFrom)
        {
            return BadRequest("Invalid booking period is provided.");
        }       
        if (bookingCreateDTO.BookingFrom < DateTimeOffset.UtcNow)
        {
            return BadRequest("Booking date must be in the future.");
        }
        if (bookingCreateDTO.BookingTo.Date != bookingCreateDTO.BookingFrom.Date)
        {
            return BadRequest("Booking From and To are not on the same day.");
        }
        var bookings = _bookingService.BookRoom(bookingCreateDTO);
        if (bookings == null)
        {
            return BadRequest("Booking is not made successfully. The following could be the reason:-\n 1. Room or user is invalid.\n 2. The room is unavailable.");
        }
        return Ok(bookings);
    }

    [HttpGet]
    public ActionResult<IEnumerable<BookingDTO>> GetBookingbyID(int id)
    {
        string message ="";
        if (id > 0) 
        {
            var booking = _bookingService.GetBookingbyID(id);
            if (booking == null)
            {
                message = $"Booking with ID, {id}, not found.";
                _logger.LogError(message);
                return NotFound(message);

            }
            return Ok(booking);
        }

         message = "Please provide valid Id for getting a booking.";
         _logger.LogError(message);
        return BadRequest(message);
    }
}