using Asp_Labb3.Data;
using Asp_Labb3.Models;
using Asp_Labb3.Models.DTOs.RequestDTOs;
using Asp_Labb3.Models.DTOs.ResponseDTOs;
using Microsoft.EntityFrameworkCore;
namespace Asp_Labb3
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			// Add services to the container.
			builder.Services.AddAuthorization();

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();

			///////////////////////
			///////// GET /////////
			///////////////////////

			//// GET - Return all users
			app.MapGet("/users", async (ApplicationDbContext context) =>
			{
				var users = await context.Users
				.Include(ui => ui.UserInterests)
				.ThenInclude(i => i.Interest)
				.ThenInclude(w => w.WebLinks)
				.ToListAsync();

				if (users == null || !users.Any())
				{
					return Results.NotFound("No users found");
				}

				// Map model to DTO
				var usersRequestDTO = users.Select(u => new UserRequestDTO
				{
					UserId = u.UserId,
					UserName = u.UserName,
					UserEmail = u.UserEmail,
					Interests = u.UserInterests.Select(ui => new InterestRequestDTO
					{
						InterestId = ui.Interest.InterestId,
						Title = ui.Interest.Title,
						Description = ui.Interest.Description,
						WebLinks = ui.Interest.WebLinks.Select(wl => new WebLinkRequestDTO
						{
							WebLinkId = wl.WebLinkId,
							Url = wl.Url,
						}).ToList(),
					}).ToList(),
				}).ToList();

				return Results.Ok(usersRequestDTO);
			});

			//// Returns all interests
			app.MapGet("/interests", async (ApplicationDbContext context) =>
			{
				var interests = await context.Interests.ToListAsync();

				if (interests.Count == 0) return Results.NotFound("No Interests Found.");

				// Map model to DTO
				var interestsResponse = interests.Select(interest => new InterestResponseDTO
				{
					InterestId = interest.InterestId,
					Title = interest.Title,
					Description = interest.Description
				}).ToList();

				return Results.Ok(interestsResponse);
			});

			//// Returns all interests from a user
			app.MapGet("/users/{userId:int}/interests", async (int userId, ApplicationDbContext context) =>
			{
				var interests = await context.Interests.Where(i => i.UserInterests.Any(ui => ui.FkUserId == userId)).ToListAsync();

				if (interests.Count == 0) return Results.NotFound($"User with ID {userId} doesn't seem to have any interests...");

				// Map model to DTO
				var interestsResponse = interests.Select(interest => new InterestResponseDTO
				{
					InterestId = interest.InterestId,
					Title = interest.Title,
					Description = interest.Description
				}).ToList();

				return Results.Ok(interestsResponse);
			});

			//// Returns all links from a user
			app.MapGet("/people/{userId:int}/interests/links", async (int userId, ApplicationDbContext context) =>
			{
				var webLinks = await context.WebLinks.Where(wl => wl.FkUserId == userId)
					.Include(i => i.Interest).ToListAsync();

				if (webLinks.Count == 0) return Results.NotFound($"User with ID {userId} has no weblinks");

				// Map model to DTO
				var weblinksResponse = webLinks.Select(wl => new WebLinkResponseDTO
				{
					WebLinkId = wl.WebLinkId,
					InterestId = wl.Interest.InterestId,
					InterestTitle = wl.Interest.Title,
					Url = wl.Url,
				}).ToList();

				return Results.Ok(weblinksResponse);
			});


			///////////////////////
			///////// POST ////////
			///////////////////////

			//// Create a user
			app.MapPost("/users", async (UserRequestDTO dto, ApplicationDbContext context) =>
			{

				// Map model to DTO
				var user = new User
				{
					UserName = dto.UserName,
					UserEmail = dto.UserEmail
				};

				await context.Users.AddAsync(user);
				await context.SaveChangesAsync();

				return Results.Created($"/users/{user.UserId}", user);
			});


			//// Create a interest
			app.MapPost("/interests", async (InterestRequestDTO dto, ApplicationDbContext context) =>
			{

				// Map model to DTO
				var interest = new Interest
				{
					Title = dto.Title,
					Description = dto.Description
				};

				await context.Interests.AddAsync(interest);
				await context.SaveChangesAsync();

				return Results.Created($"/interests/{interest.InterestId}", interest);
			});

			//// Add an interest to a user
			app.MapPost("/users/interests/", async (UserInterestRequestDTO dto, ApplicationDbContext context) =>
			{
				// Check if user and interest is valid
				var userExists = await context.Users.AnyAsync(u => u.UserId == dto.UserId);
				var interestExists = await context.Interests.AnyAsync(i => i.InterestId == dto.InterestId);

				if (!userExists || !interestExists) return Results.NotFound("User or Interest not found.");

				// Check if user already has the interest
				var userInterest = new UserInterest { FkUserId = dto.UserId, FkInterestId = dto.InterestId };
				var isDuplicate = context.UserInterests.Any(ui => ui.FkInterestId == dto.InterestId && ui.FkUserId == dto.UserId);

				if (isDuplicate) return Results.BadRequest("The user already has this interest.");

				await context.UserInterests.AddAsync(userInterest);
				await context.SaveChangesAsync();

				return Results.Created($"/users/{dto.UserId}/interests/{dto.InterestId}", userInterest);
			});

			//// Add a weblink to a user's interest
			app.MapPost("/people/interests/links", async (WebLinkRequestDTO dto, ApplicationDbContext context) =>
			{
				// Check if specified person and interest exists
				var personExists = await context.Users.AnyAsync(u => u.UserId == dto.UserId);
				var interestExists = await context.Interests.AnyAsync(i => i.InterestId == dto.InterestId);

				if (!personExists || !interestExists) return Results.NotFound("User or Interest not found.");

				// Check if person has the interest they're trying to add a link for
				var hasInterest = context.UserInterests.Any(ui => ui.FkUserId == dto.UserId && ui.FkInterestId == dto.InterestId);

				if (!hasInterest) return Results.BadRequest("The user does not have that interest.");


				// Map model to DTO
				var weblink = new WebLink
				{
					Url = dto.Url,
					FkInterestId = dto.InterestId,
					FkUserId = dto.UserId
				};

				// Add new InterestLink
				await context.WebLinks.AddAsync(weblink);
				await context.SaveChangesAsync();

				return Results.Created($"/user/{dto.UserId}/interests/{dto.InterestId}/links/{weblink.WebLinkId}", weblink);
			});

			app.Run();
		}
	}
}
