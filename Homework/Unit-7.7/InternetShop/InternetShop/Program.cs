using System;
using System.Collections.Generic;
using System.Linq;

namespace InternetShop
{
    // ВСПОМОГАТЕЛЬНЫЕ КЛАССЫ (товары, клиент)

    // Класс товара
    public class Product
    {
        public string Name { get; }
        public decimal Price { get; }

        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public override string ToString() => $"{Name} ({Price:C})";
    }

    // СИСТЕМА ДОСТАВОК

    // Используем абстракцию (базовый класс доставки), т.к. создать просто доставку нельзя, она должна быть конкретной
    public abstract class Delivery
    {
        public string Address { get; protected set; }
        public string Phone { get; set; } // Общее поле для связи

        public Delivery(string address, string phone)
        {
            Address = address;
            Phone = phone;
        }

        // Абстрактный метод: каждый тип доставки сам знает, как себя описать
        public abstract string GetDeliveryDetails();

        // Виртуальный метод: базовая логика расчета даты, которую можно переопределить
        public virtual DateTime GetEstimatedDeliveryDate()
        {
            return DateTime.Now.AddDays(3); // По умолчанию 3 дня
        }

        // Виртуальный метод: расчет стоимости доставки
        public virtual decimal CalculateShippingCost(decimal orderTotal)
        {
            return 200; // Базовая цена
        }
    }

    // Доставка на дом (курьер)
    class HomeDelivery : Delivery
    {
        // Логика, специфичная для курьера
        public string CourierServiceName { get; private set; }
        public string PreferredTimeSlot { get; set; }

        public HomeDelivery(string address, string phone, string courierService, string timeSlot)
            : base(address, phone)
        {
            CourierServiceName = courierService;
            PreferredTimeSlot = timeSlot;
        }

        public override string GetDeliveryDetails()
        {
            return $"Курьерская служба '{CourierServiceName}'. Ждать курьера: {Address}. Время: {PreferredTimeSlot}. Тел: {Phone}";
        }

        public override decimal CalculateShippingCost(decimal orderTotal)
        {
            // Если заказ дорогой, тогда доставка будет дешевле или бесплатной
            return orderTotal > 10000 ? 0 : 500;
        }
    }

    // Доставка в пункт выдачи
    class PickPointDelivery : Delivery
    {
        // Логика хранения компании и ID точки (например, CDEK, Boxberry)
        public string PickPointCompany { get; private set; }
        public string PointId { get; private set; }
        public int StorageDays { get; private set; }

        public PickPointDelivery(string address, string phone, string company, string pointId)
            : base(address, phone)
        {
            PickPointCompany = company;
            PointId = pointId;
            StorageDays = 5; // Срок храния 5 дней
        }

        public override string GetDeliveryDetails()
        {
            return $"Пункт выдачи '{PickPointCompany}' (ID: {PointId}). Адрес: {Address}. Хранение: {StorageDays} дн.";
        }

        public override decimal CalculateShippingCost(decimal orderTotal)
        {
            return 150; // Фиксированная цена для постаматов
        }
    }

    // Доставка в розничный магазин (самовывоз)
    class ShopDelivery : Delivery
    {
        // Внутренняя логика
        public int ShopId { get; private set; }
        public string ShopOpeningHours { get; private set; }

        // Для магазина адрес часто фиксированный, но передадим его в конструктор
        public ShopDelivery(string address, string phone, int shopId)
            : base(address, phone)
        {
            ShopId = shopId;
            ShopOpeningHours = "09:00 - 21:00";
        }

        public override string GetDeliveryDetails()
        {
            return $"Самовывоз из магазина #{ShopId}. Адрес: {Address}. Часы работы: {ShopOpeningHours}.";
        }

        public override DateTime GetEstimatedDeliveryDate()
        {
            return DateTime.Now.AddDays(1); // В магазин привозят быстрее
        }

        public override decimal CalculateShippingCost(decimal orderTotal)
        {
            return 0; // Самовывоз бесплатный
        }
    }

    // ЗАКАЗЫ

    // Базовый не дженерик класс, чтобы хранить все заказы в одном списке
    abstract class OrderBase
    {
        public int Number { get; protected set; }
        public List<Product> Products { get; protected set; } = new List<Product>();

        // Свойство только для чтения, возвращающее общую сумму
        public decimal TotalPrice => Products.Sum(p => p.Price);

        public abstract void DisplayInfo();
    }

    // Класс заказа
    class Order<TDelivery> : OrderBase where TDelivery : Delivery
    {
        public TDelivery Delivery { get; private set; }
        public string Description { get; set; }

        public Order(int number, string description, TDelivery delivery)
        {
            Number = number;
            Description = description;
            Delivery = delivery;
        }

        public void AddProduct(Product product)
        {
            Products.Add(product);
        }

        // Реализация метода отображения с использованием полиморфизма Delivery
        public override void DisplayInfo()
        {
            decimal shippingCost = Delivery.CalculateShippingCost(TotalPrice);
            DateTime date = Delivery.GetEstimatedDeliveryDate();

            Console.WriteLine("========================================");
            Console.WriteLine($"Заказ №{Number}: {Description}");
            Console.WriteLine("Товары:");
            foreach (var p in Products)
            {
                Console.WriteLine($" - {p}");
            }
            Console.WriteLine($"Сумма товаров: {TotalPrice:C}");
            Console.WriteLine($"Доставка: {shippingCost:C} (Итого: {TotalPrice + shippingCost:C})");
            Console.WriteLine($"Дата доставки: {date.ToShortDateString()}");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Детали доставки: {Delivery.GetDeliveryDetails()}");
            Console.ResetColor();
            Console.WriteLine("========================================\n");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Создаем коллекцию для хранения любых заказов
            var orders = new List<OrderBase>();

            // Заказ 1: Доставка на дом
            var homeDelivery = new HomeDelivery(
                address: "ул. Пушкина, д. 10, кв. 5",
                phone: "+79001112233",
                courierService: "Yandex Go",
                timeSlot: "18:00 - 20:00"
            );

            var order1 = new Order<HomeDelivery>(101, "Подарок на Новый год", homeDelivery);
            order1.AddProduct(new Product("Смартфон Samsung Galaxy A56", 29990));
            order1.AddProduct(new Product("Защитный чехол", 2000));

            // Заказ 2: Постамат
            var pickPoint = new PickPointDelivery(
                address: "ТЦ 'Фантастика', 1 этаж",
                phone: "+79998887766",
                company: "Boxberry",
                pointId: "NN-1234"
            );

            var order2 = new Order<PickPointDelivery>(102, "Книги и канцелярия", pickPoint);
            order2.AddProduct(new Product("Книга 'C#. Алгоритмы и структуры данных'", 3500));

            // Заказ 3: Магазин (самовывоз)
            var shopDelivery = new ShopDelivery(
                address: "пр-кт Гагарина, д.29, 1 этаж (МАГАЗИН)",
                phone: "+78005553535",
                shopId: 77
            );

            var order3 = new Order<ShopDelivery>(103, "Комплектующие для ПК", shopDelivery);
            order3.AddProduct(new Product("Видеокарта GeForce RTX 5070", 63000));

            // Добавляем все заказы в общий список
            orders.Add(order1);
            orders.Add(order2);
            orders.Add(order3);

            // Выводим информацию
            foreach (var order in orders)
            {
                order.DisplayInfo();
            }

            Console.ReadKey();
        }
    }
}