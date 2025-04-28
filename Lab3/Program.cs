using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SocialNetworkLINQ
{
    class Program
    {
            private static readonly Dictionary<string, Action> Commands = new Dictionary<string, Action>
{
    { "1", GenerateTestData },
    { "2", ProjectionPostsWithAuthors },
    { "3", GroupUsersByCity },
    { "4", PostsAggregation },
    { "5", AllFriendRequests },
    { "6", CheckConditions },
    { "7", UsersWithoutPosts },
    { "8", TopPopularPosts },
    { "0", () => { Console.WriteLine("Выход из программы."); Environment.Exit(0); } },
    { "pause", () => { Console.WriteLine("\nНажмите любую клавишу для продолжения..."); Console.ReadKey(); } }

};
        static string connectionString = "Server=Jacob-book\\SQLEXPRESS;Database=TestAppDb;User Id=sa;Password=9052;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            try
            {
                bool exit = false;
                while (!exit)
                {
                    Console.Clear();
                    Console.WriteLine("=== СОЦИАЛЬНАЯ СЕТЬ - LINQ ЗАПРОСЫ ===");
                    Console.WriteLine("1. Создать/пересоздать тестовые данные");
                    Console.WriteLine("2. Проекция: посты с авторами");
                    Console.WriteLine("3. Группировка: пользователи по городам");
                    Console.WriteLine("4. Агрегация: статистика по постам");
                    Console.WriteLine("5. Объединение: все запросы на дружбу");
                    Console.WriteLine("6. Проверка условий");
                    Console.WriteLine("7. Поиск пользователей без постов");
                    Console.WriteLine("8. Топ-5 популярных постов");
                    Console.WriteLine("0. Выход");
                    Console.Write("Выберите действие: ");

                    string input = Console.ReadLine();

        if (Commands.TryGetValue(input, out var command))
        {
            command();
            if (input != "0")
                {
                    Commands["pause"]();
                }
        }
        
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void GenerateTestData()
        {
            Console.WriteLine("Тестовые данные успешно созданы!");
            Console.WriteLine($"Пользователей: {CountRecords("Users")}");
            Console.WriteLine($"Постов: {CountRecords("Posts")}");
            Console.WriteLine($"Запросов на дружбу: {CountRecords("FriendRequests")}");
        }

        static void ProjectionPostsWithAuthors()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var users = GetUsers(connection);
                var posts = GetPosts(connection);

                var result = posts
                    .Join(users,
                        post => post.UserId,
                        user => user.UserId,
                        (post, user) => new
                        {
                            Author = user.Name,
                            Preview = post.Content.Length > 20 ? post.Content.Substring(0, 20) + "..." : post.Content,
                            Likes = post.Likes,
                            Date = post.CreatedDate.ToString("dd.MM.yyyy")
                        })
                    .OrderByDescending(x => x.Likes)
                    .Take(10);

                Console.WriteLine("=== Топ-10 постов с авторами ===");
                Console.WriteLine("{0,-20} {1,-30} {2,-10} {3}", "Автор", "Превью", "Лайки", "Дата");
                foreach (var item in result)
                {
                    Console.WriteLine("{0,-20} {1,-30} {2,-10} {3}", item.Author, item.Preview, item.Likes, item.Date);
                }
                
            }
        }

        static void GroupUsersByCity()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var users = GetUsers(connection);

                var result = users
                    .GroupBy(u => u.City)
                    .Select(g => new
                    {
                        City = g.Key,
                        Count = g.Count(),
                        AvgAge = g.Average(u => u.Age),
                        Youngest = g.Min(u => u.Age),
                        Oldest = g.Max(u => u.Age)
                    })
                    .OrderByDescending(g => g.Count);

                Console.WriteLine("=== Пользователи по городам ===");
                Console.WriteLine("{0,-20} {1,-10} {2,-10} {3,-10} {4}", "Город", "Кол-во", "Ср.возр", "Мин", "Макс");
                foreach (var group in result)
                {
                    Console.WriteLine("{0,-20} {1,-10} {2,-10:F1} {3,-10} {4}",
                        group.City, group.Count, group.AvgAge, group.Youngest, group.Oldest);
                }
                
            }
        }

        static void PostsAggregation()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var posts = GetPosts(connection);

                Console.WriteLine("=== Статистика по постам ===");
                Console.WriteLine($"Всего постов: {posts.Count()}");
                Console.WriteLine($"Макс лайков: {posts.Max(p => p.Likes)}");
                Console.WriteLine($"Мин лайков: {posts.Min(p => p.Likes)}");
                Console.WriteLine($"Среднее лайков: {posts.Average(p => p.Likes):F1}");
                Console.WriteLine($"Сумма лайков: {posts.Sum(p => p.Likes)}");

                var byYear = posts
                    .GroupBy(p => p.CreatedDate.Year)
                    .Select(g => new { Year = g.Key, Count = g.Count(), AvgLikes = g.Average(p => p.Likes) });

                Console.WriteLine("\nПо годам:");
                foreach (var year in byYear)
                {
                    Console.WriteLine($"{year.Year}: {year.Count} постов, в среднем {year.AvgLikes:F1} лайков");
                }
                
            }
        }

        static void AllFriendRequests()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var requests = GetFriendRequests(connection);
                var users = GetUsers(connection);

                var result = requests
                    .Join(users,
                        r => r.FromUserId,
                        u => u.UserId,
                        (r, u) => new { Request = r, FromUser = u })
                    .Join(users,
                        temp => temp.Request.ToUserId,
                        u => u.UserId,
                        (temp, toUser) => new
                        {
                            RequestId = temp.Request.RequestId,
                            FromUser = temp.FromUser.Name,
                            ToUser = toUser.Name,
                            Status = temp.Request.Status,
                            Date = temp.Request.CreatedDate.ToString("dd.MM.yyyy")
                        })
                    .OrderByDescending(x => x.Date)
                    .Take(15);

                Console.WriteLine("=== Последние 15 запросов на дружбу ===");
                Console.WriteLine("{0,-5} {1,-20} {2,-20} {3,-10} {4}", "ID", "Отправитель", "Получатель", "Статус", "Дата");
                foreach (var item in result)
                {
                    Console.WriteLine("{0,-5} {1,-20} {2,-20} {3,-10} {4}",
                        item.RequestId, item.FromUser, item.ToUser, item.Status, item.Date);
                }
                
            }
        }

        static void CheckConditions()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var posts = GetPosts(connection);

                bool allHaveLikes = posts.All(p => p.Likes > 0);
                bool anyViral = posts.Any(p => p.Likes > 400);
                int popularCount = posts.Count(p => p.Likes > 100);

                Console.WriteLine("=== Проверка условий ===");
                Console.WriteLine($"Все посты имеют хотя бы 1 лайк: {allHaveLikes}");
                Console.WriteLine($"Есть вирусные посты (>400 лайков): {anyViral}");
                Console.WriteLine($"Популярных постов (>100 лайков): {popularCount} из {posts.Count()}");

                var topPost = posts.OrderByDescending(p => p.Likes).FirstOrDefault();
                if (topPost != null)
                {
                    Console.WriteLine($"\nСамый популярный пост: {topPost.Likes} лайков");
                    Console.WriteLine($"Содержание: {(topPost.Content.Length > 50 ? topPost.Content.Substring(0, 50) + "..." : topPost.Content)}");
                }
                
            }
        }

        static void UsersWithoutPosts()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var users = GetUsers(connection);
                var posts = GetPosts(connection);

                var usersWithPosts = users
                    .GroupJoin(posts,
                        user => user.UserId,
                        post => post.UserId,
                        (user, userPosts) => new { User = user, PostCount = userPosts.Count() })
                    .Where(x => x.PostCount == 0)
                    .Select(x => x.User);

                Console.WriteLine("=== Пользователи без постов ===");
                Console.WriteLine("{0,-5} {1,-20} {2,-10} {3}", "ID", "Имя", "Возраст", "Город");
                foreach (var user in usersWithPosts)
                {
                    Console.WriteLine("{0,-5} {1,-20} {2,-10} {3}", user.UserId, user.Name, user.Age, user.City);
                }
                Console.WriteLine($"\nВсего: {usersWithPosts.Count()} пользователей без постов");
                
            }
        }

        static void TopPopularPosts()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var users = GetUsers(connection);
                var posts = GetPosts(connection);

                var result = posts
                    .Join(users,
                        post => post.UserId,
                        user => user.UserId,
                        (post, user) => new
                        {
                            PostId = post.PostId,
                            Author = user.Name,
                            City = user.City,
                            Content = post.Content.Length > 30 ? post.Content.Substring(0, 30) + "..." : post.Content,
                            Likes = post.Likes,
                            DaysOld = (DateTime.Now - post.CreatedDate).Days
                        })
                    .OrderByDescending(x => x.Likes)
                    .ThenBy(x => x.DaysOld)
                    .Take(5);

                Console.WriteLine("=== Топ-5 популярных постов ===");
                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-35} {4,-10} {5}", 
                    "ID", "Автор", "Город", "Содержание", "Лайки", "Дней назад");
                foreach (var post in result)
                {
                    Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-35} {4,-10} {5}",
                        post.PostId, post.Author, post.City, post.Content, post.Likes, post.DaysOld);
                }
                
            }
        }
#region Вспомогательные классы
        static List<User> GetUsers(SqlConnection connection)
        {
            var users = new List<User>();
            using (var command = new SqlCommand("SELECT * FROM Users", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        UserId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Age = reader.GetInt32(2),
                        City = reader.GetString(3),
                        RegistrationDate = reader.GetDateTime(4)
                    });
                }
            }
            return users;
        }

        static List<Post> GetPosts(SqlConnection connection)
        {
            var posts = new List<Post>();
            using (var command = new SqlCommand("SELECT * FROM Posts", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    posts.Add(new Post
                    {
                        PostId = reader.GetInt32(0),
                        UserId = reader.GetInt32(1),
                        Content = reader.GetString(2),
                        Likes = reader.GetInt32(3),
                        CreatedDate = reader.GetDateTime(4)
                    });
                }
            }
            return posts;
        }

        static List<FriendRequest> GetFriendRequests(SqlConnection connection)
        {
            var requests = new List<FriendRequest>();
            using (var command = new SqlCommand("SELECT * FROM FriendRequests", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    requests.Add(new FriendRequest
                    {
                        RequestId = reader.GetInt32(0),
                        FromUserId = reader.GetInt32(1),
                        ToUserId = reader.GetInt32(2),
                        Status = reader.GetString(3),
                        CreatedDate = reader.GetDateTime(4)
                    });
                }
            }
            return requests;
        }

        static int CountRecords(string tableName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand($"SELECT COUNT(*) FROM {tableName}", connection))
                {
                    return (int)command.ExecuteScalar();
                }
            }
        }

        #endregion

        #region Классы моделей

        class User
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string City { get; set; }
            public DateTime RegistrationDate { get; set; }
        }

        class Post
        {
            public int PostId { get; set; }
            public int UserId { get; set; }
            public string Content { get; set; }
            public int Likes { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        class FriendRequest
        {
            public int RequestId { get; set; }
            public int FromUserId { get; set; }
            public int ToUserId { get; set; }
            public string Status { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        #endregion
    }
}