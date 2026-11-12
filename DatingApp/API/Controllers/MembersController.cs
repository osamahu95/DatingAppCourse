using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MembersController(IMemberRepository membersRepository, IPhotoService photoService) : BaseApiController
    {
        private readonly IMemberRepository membersRepository = membersRepository;
        private readonly IPhotoService photoService = photoService;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            var members = await membersRepository.GetMembersAsync();
            return Ok(members);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            var member = await membersRepository.GetMemberByIdAsync(id);

            if (member == null) return NotFound();

            return member;
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
        {
            var memberPhotos = await membersRepository.GetPhotosForMemberAsync(id);
            return Ok(memberPhotos);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            var memberId = User.GetMemberId();

            var member = await membersRepository.GetMemberforUpdate(memberId);
            if (member == null) return BadRequest("Could not get Member");

            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description = memberUpdateDto.Description ?? member.Description;
            member.City = memberUpdateDto.City ?? member.City;
            member.Country = memberUpdateDto.Country ?? member.Country;

            if (member.User != null)
                member.User.DisplayName = memberUpdateDto.DisplayName ?? member.User.DisplayName;

            membersRepository.Update(member); // optional 
            if (await membersRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to Update Member");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo>> AddPhoto([FromForm] IFormFile file)
        {
            var member = await membersRepository.GetMemberforUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Cannot Update Member");

            var result = await photoService.UploadPhotoAsync(file);
            if(result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                MemberId = User.GetMemberId()
            };

            if (member.ImageUrl == null)
            {
                member.ImageUrl = photo.Url;
            }

            member.Photos.Add(photo);

            if (await membersRepository.SaveAllAsync()) return photo;

            return BadRequest("Problem adding Photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var member = await membersRepository.GetMemberforUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Cannot get member from token");

            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

            if (member.ImageUrl == photo?.Url || photo == null)
            {
                return BadRequest("Cannot set this as main Image");
            }

            member.ImageUrl = photo.Url;

            if (await membersRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Problem setting main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var member = await membersRepository.GetMemberforUpdate(User.GetMemberId());
            if (member == null) return BadRequest("Cannot get member from Token");

            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);
            if (photo == null || photo.Url == member.ImageUrl)
            {
                return BadRequest("This Photo cannot be deleted");
            }

            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            member.Photos.Remove(photo);
            if (await membersRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the Photo");
        }
    }
}
