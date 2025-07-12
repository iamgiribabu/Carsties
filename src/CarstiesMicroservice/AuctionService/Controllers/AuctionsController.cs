using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;

        public AuctionsController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAuctions()
        {
            var auctions = await _context.Auctions.Include(a => a.Item).OrderBy(a => a.Item.Make).ToListAsync();
            var auctionDtos = _mapper.Map<List<AuctionDto>>(auctions);
            return Ok(auctionDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
        {
            var auction = await _context.Auctions.Include(a => a.Item).FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
            {
                return NotFound();
            }
            var auctionDto = _mapper.Map<AuctionDto>(auction);
            return Ok(auctionDto);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            var auction = _mapper.Map<Auction>(createAuctionDto);
            auction.Item = _mapper.Map<Item>(createAuctionDto);
            // ToDo: add current user as seller
            auction.Seller = "currentUser"; // Replace with actual user retrieval logic

            _context.Auctions.Add(auction);
            var result = await _context.SaveChangesAsync() > 0;
            if (!result)
            {
                return BadRequest("Failed to create auction");
            }
            var auctionDto = _mapper.Map<AuctionDto>(auction);
            return CreatedAtAction(nameof(GetAuction), new { id = auction.Id }, auctionDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await _context.Auctions.Include(a => a.Item).FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
            {
                return NotFound();
            }

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Year = updateAuctionDto.Year ??  auction.Item.Year;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;

            //_mapper.Map(updateAuctionDto, auction);
            //_mapper.Map(updateAuctionDto, auction.Item);
            //_context.Auctions.Update(auction);
            var result = await _context.SaveChangesAsync() > 0;
            if (!result)
            {
                return BadRequest("Failed to update auction");
            }
            //var auctionDto = _mapper.Map<AuctionDto>(auction);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            _context.Auctions.Remove(auction);
            var result = await _context.SaveChangesAsync() > 0;
            if (!result)
            {
                return BadRequest("Failed to delete auction");
            }
            return NoContent();
        }
    }
}
