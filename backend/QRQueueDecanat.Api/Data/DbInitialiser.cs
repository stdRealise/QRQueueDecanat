using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using QRQueueDecanat.Entities;

namespace QRQueueDecanat.Data;

// Для демонстрации
public static class DbInitialiser
{   
     public static async Task SeedAsync(ApplicationDbContext context,
        IPasswordHasher<AppUser> passwordHasher,
        CancellationToken cancellationToken = default)
    {
        await SeedServicesAsync(context, cancellationToken);
        await SeedCountersAsync(context, cancellationToken);
        await SeedUsersAsync(context, passwordHasher, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedServicesAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        if (await context.Services.AnyAsync(cancellationToken))
        {
            return; 
        }
        var services = new List<Service>
        {
            new()
            {
                Name = "Получение справок",
                Prefix = "A",
                Minutes = 5,
                IconKey = "document",
                IsActive = true
            },
            new()
            {
                Name = "Подпись документов",
                Prefix = "B",
                Minutes = 5,
                IconKey = "signature",
                IsActive = true
            },
            new()
            {
                Name = "Академический отпуск",
                Prefix = "C",
                Minutes = 10,
                IconKey = "calendar",
                IsActive = true
            },
            new()
            {
                Name = "Перевод/Восстановление",
                Prefix = "D",
                Minutes = 15,
                IconKey = "transfer",
                IsActive = true
            },
            new()
            {
                Name = "Общежитие",
                Prefix = "E",
                Minutes = 15,
                IconKey = "dormitory",
                IsActive = true
            },
            new()
            {
                Name = "Стипендия",
                Prefix = "F",
                Minutes = 15,
                IconKey = "scholarship",
                IsActive = true
            },
            new()
            {
                Name = "Консультация",
                Prefix = "G",
                Minutes = 15,
                IconKey = "consultation",
                IsActive = true
            }
        };
        context.Services.AddRange(services);
    }

    private static async Task SeedCountersAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        if (await context.Counters.AnyAsync(cancellationToken))
        {
            return; 
        }
        var counters = Enumerable
            .Range(1, 8)
            .Select(number => new Counter
            {
                Number = number,
                IsActive = true
            })
            .ToList();
        context.Counters.AddRange(counters);
    }

    private static async Task SeedUsersAsync(
        ApplicationDbContext context,
        IPasswordHasher<AppUser> passwordHasher,
        CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(cancellationToken))
        {
            return; 
        }
        var users = new List<AppUser>
        {
            new AppUser
            {
                Username = "admin",
                PasswordHash = string.Empty,
                FullName = "Админ Админ Админ",
                RoleId = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new AppUser
            {
                Username = "operator1",
                PasswordHash = string.Empty,
                FullName = "Иванова Мария Ивановна",
                RoleId = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new AppUser
            {
                Username = "operator2",
                PasswordHash = string.Empty,
                FullName = "Петрова Людмила Сергеевна",
                RoleId = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };
        var passwords = new[]
        {
            "Admin123!",
            "Operator123!",
            "Operator123!"
        };
        for (var i = 0; i < users.Count; i++)
        {
            var newUser = users[i];
            var password = passwords[i];
            newUser.PasswordHash = passwordHasher.HashPassword(
                newUser, password);
            context.Users.Add(newUser);
        }
    }
}