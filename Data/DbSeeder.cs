using AliMertOyunMagaza.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AliMertOyunMagaza.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDefaultDataAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (await userManager.FindByEmailAsync("admin@gmail.com") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FullName = "Sistem Yöneticisi"
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Kategoriler
            if (!context.Categories.Any())
            {
                var ryo = new Category { Name = "RYO", Description = "Rol Yapma Oyunları" };
                var aksiyon = new Category { Name = "Aksiyon", Description = "Aksiyon ve Macera Oyunları" };
                var fps = new Category { Name = "FPS", Description = "Birinci Şahıs Nişancı Oyunları" };

                context.Categories.AddRange(ryo, aksiyon, fps);
                await context.SaveChangesAsync();
            }

            // Eğer oyun sayısı 24'ten azsa, var olanları temizle ve yeni paketi ekle.
            if (context.Games.Count() < 24)
            {
                var ryoId = (await context.Categories.FirstOrDefaultAsync(c => c.Name == "RYO"))?.Id ?? 0;
                var aksiyonId = (await context.Categories.FirstOrDefaultAsync(c => c.Name == "Aksiyon"))?.Id ?? 0;
                var fpsId = (await context.Categories.FirstOrDefaultAsync(c => c.Name == "FPS"))?.Id ?? 0;

                // Eski kayıtlar temizlenir
                var existingWishlist = context.WishlistItems.ToList();
                context.WishlistItems.RemoveRange(existingWishlist);
                
                var existingGames = context.Games.ToList();
                context.Games.RemoveRange(existingGames);
                
                await context.SaveChangesAsync();

                // 8 Adet RYO
                context.Games.AddRange(
                    new Game { Title = "The Witcher 3: Wild Hunt", Description = "Efsanevi bir RYO macerası ve bitmeyen görevler stili.", Price = 250.00m, CategoryId = ryoId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1wyy.jpg" },
                    new Game { Title = "Skyrim", Description = "Ejderhalar, büyü ve her köşesinde farklı bir sır saklayan sonsuz bir dünya.", Price = 150.00m, CategoryId = ryoId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1tnw.jpg" },
                    new Game { Title = "Final Fantasy VII Remake", Description = "Eski usul klasik bir JRYO deneyiminin baştan aşağı yenilenmiş modern hali.", Price = 600.00m, CategoryId = ryoId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1qxr.jpg" },
                    new Game { Title = "Persona 5 Royal", Description = "Gündüzleri Tokyo'da sıradan bir lise öğrencisi, geceleri ise hayalet kalplerin hırsızı.", Price = 800.00m, CategoryId = ryoId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1nic.jpg" },
                    new Game { Title = "Baldur's Gate 3", Description = "Gerçek bir D&D deneyimini masadan ekrana taşıyan yılın fantezi oyunu.", Price = 799.00m, CategoryId = ryoId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co670h.jpg" },
                    new Game { Title = "Elden Ring", Description = "Devasa açık dünya keşfi ve zorlu boss savaşlarıyla bezeli karanlık fantezi.", Price = 999.00m, CategoryId = ryoId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co4jni.jpg" },
                    new Game { Title = "Mass Effect", Description = "Uzayın derinliklerinde yıldızlararası diplomatik sırlar saklayan bilim kurgu efsanesi.", Price = 450.00m, CategoryId = ryoId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2lba.jpg" },
                    new Game { Title = "Fallout 4", Description = "Nükleer kıyamet sonrası çorak topraklarda hayatta kalma mücadelesi RYO'su.", Price = 175.00m, CategoryId = ryoId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2k32.jpg" }
                );

                // 8 Adet Aksiyon
                context.Games.AddRange(
                    new Game { Title = "Cyberpunk 2077", Description = "Neon ışıkları altındaki dev Night City'de geçen aksiyon dolu açık dünya deneyimi.", Price = 499.00m, CategoryId = aksiyonId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2mvt.jpg" },
                    new Game { Title = "Red Dead Redemption 2", Description = "Acımasız Vahşi Batı'nın çöküş döneminde geçen destansı açık dünya soygun macerası.", Price = 1200.00m, CategoryId = aksiyonId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1q1f.jpg" },
                    new Game { Title = "Grand Theft Auto V", Description = "Suç dünyasına adım attığınız ve sınır tanımayan küresel fenomen kült oyun.", Price = 300.00m, CategoryId = aksiyonId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2lbd.jpg" }, 
                    new Game { Title = "God of War", Description = "Soğuk İskandinav mitolojisi evreninde bir babanın ve oğlunun intikam dolu dokunaklı hikayesi.", Price = 650.00m, CategoryId = aksiyonId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1tmu.jpg" },
                    new Game { Title = "Marvel's Spider-Man", Description = "Gösterişli ağ atma mekanikleri ile New York gökdelenleri arasında dolaşma keyfi.", Price = 500.00m, CategoryId = aksiyonId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1r7f.jpg" },
                    new Game { Title = "Uncharted 4", Description = "Gözüpek hazine avcısı Nathan Drake'in görsel şölen eşliğindeki tehlikeli son gezisi.", Price = 450.00m, CategoryId = aksiyonId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1r7m.jpg" },
                    new Game { Title = "Assassin's Creed Valhalla", Description = "Viking çağının baltalı çarpışmalarını ve yeni diyarlara yelken açmayı deneyimleyin.", Price = 700.00m, CategoryId = aksiyonId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2p7d.jpg" },
                    new Game { Title = "Devil May Cry 5", Description = "Dante ve Nero ile beraber stilize aksiyon komboları atarak şeytan avlayın.", Price = 400.00m, CategoryId = aksiyonId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1rc4.jpg" }
                );

                // 8 Adet FPS
                context.Games.AddRange(
                    new Game { Title = "Doom Eternal", Description = "Cehennem ordularına karşı hiç yavaşlamak bilmeyen hızlı tempolu şeytan avlama şöleni.", Price = 199.00m, CategoryId = fpsId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1osj.jpg" },
                    new Game { Title = "Call of Duty: MW II", Description = "Savaş alanının karmaşasını sunan yepyeni grafiklere sahip modern savaş simülasyonu.", Price = 1100.00m, CategoryId = fpsId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co4y1s.jpg" },
                    new Game { Title = "Counter-Strike 2", Description = "Taktiksel ekip çalışması gerektiren rekabetçi nişancı aksiyon oyunlarının süregelen atası.", Price = 0.00m, CategoryId = fpsId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co6f7s.jpg" },
                    new Game { Title = "Valorant", Description = "Her turda yeteneklerini konuşturan, ajan ve beceri odaklı taktiksel takım savaşları.", Price = 0.00m, CategoryId = fpsId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2mvt.jpg" }, 
                    new Game { Title = "Apex Legends", Description = "Fütüristik bir gezegende kahramanların özel yeteneklerini sergilediği hızlı battle royale.", Price = 0.00m, CategoryId = fpsId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co1wdq.jpg" },
                    new Game { Title = "Battlefield 2042", Description = "Savaş mekaniklerini, araç kullanımını ve atmosferi hissettiren geniş çaplı modern çatışma.", Price = 800.00m, CategoryId = fpsId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co36x2.jpg" },
                    new Game { Title = "Overwatch 2", Description = "Amacın uyum olduğu karakter kartlarıyla oluşturulan renkli tabanlı çok oyunculu shooter.", Price = 0.00m, CategoryId = fpsId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co56ks.jpg" },
                    new Game { Title = "Halo Infinite", Description = "Spartan zırhını kuşanan Master Chief'in geniş açık dünyada devam eden serüveni.", Price = 300.00m, CategoryId = fpsId, ImageUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co42r4.jpg" }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
