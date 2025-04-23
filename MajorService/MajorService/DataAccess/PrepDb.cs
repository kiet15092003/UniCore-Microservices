using MajorService.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace MajorService.DataAccess
{
    public static class PrepDb
    {
        public static async Task PrepPopulationAsync(IApplicationBuilder app, bool isProduction)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                await SeedDataAsync(context, isProduction);
            }
        }

        private static async Task SeedDataAsync(AppDbContext context, bool isProduction)
        {
            await context.Database.MigrateAsync();

            await SeedMajorsAsync(context);
        }
        private static async Task SeedMajorsAsync(AppDbContext context)
        {
            if (!await context.Departments.AnyAsync())
            {
                var department = new Department
                {
                    Name = "Information Technology",
                    Code = "IT"
                };

                await context.Departments.AddAsync(department);
                await context.SaveChangesAsync(); // Save to get the generated ID

                Console.WriteLine($"Department Created: {department.Id}, Name: {department.Name}, Code: {department.Code}");

                var majorGroups = new List<MajorGroup>
                {
                    new MajorGroup
                    {
                        Name = "Software Engineering",
                        DepartmentId = department.Id, // Link to Department
                    },
                    new MajorGroup
                    {
                        Name = "Computer Science",
                        DepartmentId = department.Id, // Link to Department
                    }
                };

                await context.MajorGroups.AddRangeAsync(majorGroups);
                await context.SaveChangesAsync(); // Save to get the generated IDs

                Console.WriteLine("Major Groups Created:");
                foreach (var mg in majorGroups)
                {
                    Console.WriteLine($"ID: {mg.Id}, Name: {mg.Name}, DepartmentId: {mg.DepartmentId}");
                }

                var majors = new List<Major>
                {
                    new Major { Name = "Software Engineering High Quality", Code = "SE2", MajorGroupId = majorGroups[0].Id },
                    new Major { Name = "Software Engineering Normal", Code = "SE1", MajorGroupId = majorGroups[0].Id },
                    new Major { Name = "Computer Science High Quality", Code = "CS2", MajorGroupId = majorGroups[1].Id },
                    new Major { Name = "Computer Science Normal", Code = "CS1", MajorGroupId = majorGroups[1].Id }
                };

                await context.Majors.AddRangeAsync(majors);
                await context.SaveChangesAsync();

                Console.WriteLine("Majors Created:");
                foreach (var major in majors)
                {
                    Console.WriteLine($"ID: {major.Id}, Name: {major.Name}, Code: {major.Code}, MajorGroupId: {major.MajorGroupId}");
                }

                Console.WriteLine("Seeding Completed.");
            }
        }
    }
}
