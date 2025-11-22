using System;

namespace UserProfileApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // 3. Вызов методов из метода Main
            var userData = GetUserData();
            ShowUserData(userData);

            // Чтобы консоль не закрылась сразу
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        // 1. Метод для сбора данных (возвращает кортеж)
        static (string Name, string LastName, int Age, string[] PetNames, string[] FavColors) GetUserData()
        {
            // Ввод имени
            Console.Write("Введите имя: ");
            string name = Console.ReadLine();
            // Простая проверка, чтобы имя не было пустым
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Имя не может быть пустым. Попробуйте еще раз.");
                name = Console.ReadLine();
            }

            // Ввод фамилии
            Console.Write("Введите фамилию: ");
            string lastName = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(lastName))
            {
                Console.WriteLine("Фамилия не может быть пустой. Попробуйте еще раз.");
                lastName = Console.ReadLine();
            }

            // Ввод возраста (с проверкой через отдельный метод)
            int age = CheckCorrectInput("Введите ваш возраст: ");

            // Данные о питомце
            Console.Write("Есть ли у вас питомец? (Да/Нет): ");
            string hasPetInput = Console.ReadLine().ToLower();

            string[] petNames = new string[0]; // По умолчанию массив пустой

            // Проверяем ответ пользователя (учитываем русские и английские варианты)
            if (hasPetInput == "да" || hasPetInput == "yes" || hasPetInput == "y")
            {
                int petsCount = CheckCorrectInput("Введите количество питомцев: ");
                // Вызов метода для заполнения массива кличек
                petNames = CreateStringArray(petsCount, "кличку питомца");
            }

            // Данные о цветах
            int favColorsCount = CheckCorrectInput("Введите количество любимых цветов: ");
            // Вызов метода для заполнения массива цветов
            string[] favColors = CreateStringArray(favColorsCount, "любимый цвет");

            // Возвращаем кортеж
            return (name, lastName, age, petNames, favColors);
        }

        // 2. Метод проверки корректности ввода чисел (int > 0)
        static int CheckCorrectInput(string message)
        {
            int result;
            bool isValid = false;

            do
            {
                Console.Write(message);
                string input = Console.ReadLine();

                // Проверяем: удалось ли преобразовать в int И число больше 0
                if (int.TryParse(input, out result) && result > 0)
                {
                    isValid = true;
                }
                else
                {
                    Console.WriteLine("Данные некорректны! Введите целое число больше 0.");
                }

            } while (!isValid);

            return result;
        }

        // Вспомогательный метод для создания и заполнения массива строк (для питомцев и цветов)
        static string[] CreateStringArray(int count, string objectName)
        {
            string[] array = new string[count];
            for (int i = 0; i < count; i++)
            {
                Console.Write($"Введите {objectName} №{i + 1}: ");
                array[i] = Console.ReadLine();
            }
            return array;
        }

        // 3. Метод для вывода данных на экран
        static void ShowUserData((string Name, string LastName, int Age, string[] PetNames, string[] FavColors) user)
        {
            Console.WriteLine("\n---------------------------");
            Console.WriteLine("    АНКЕТА ПОЛЬЗОВАТЕЛЯ    ");
            Console.WriteLine("---------------------------");

            Console.WriteLine($"Имя: {user.Name}");
            Console.WriteLine($"Фамилия: {user.LastName}");
            Console.WriteLine($"Возраст: {user.Age}");

            Console.WriteLine("---------------------------");
            if (user.PetNames.Length > 0)
            {
                Console.WriteLine("Питомцы:");
                foreach (var pet in user.PetNames)
                {
                    Console.WriteLine($" - {pet}");
                }
            }
            else
            {
                Console.WriteLine("Питомцев нет.");
            }

            Console.WriteLine("---------------------------");
            Console.WriteLine("Любимые цвета:");
            foreach (var color in user.FavColors)
            {
                Console.WriteLine($" - {color}");
            }
        }
    }
}