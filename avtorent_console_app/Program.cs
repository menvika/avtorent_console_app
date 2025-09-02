using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


public class Program
{
    static void Main(string[] args)
    {
        try
        {
            Application app = new Application();
            app.Run();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Критическая ошибка: {ex.Message}");
            Console.ResetColor();
        }
    }
}

public class Application
{
    private readonly OrderManager _orderManager;
    private readonly DriverManager _driverManager;
    private readonly CarManager _carManager;

    public Application()
    {
        _orderManager = new OrderManager();
        _driverManager = new DriverManager();
        _carManager = new CarManager();
    }

    public void Run()
    {

        while (true)
        {
            try
            {
                Console.WriteLine("\nГлавное меню:");
                Console.WriteLine("1) Управление заказами");
                Console.WriteLine("2) Управление водителями");
                Console.WriteLine("3) Управление автомобилями");
                Console.WriteLine("4) Выход");
                Console.Write("Выберите действие: ");

                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input) || !int.TryParse(input, out int choice))
                {
                    ShowError("Некорректный ввод. Введите число от 1 до 4.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        _orderManager.ShowMenu();
                        break;
                    case 2:
                        _driverManager.ShowMenu();
                        break;
                    case 3:
                        _carManager.ShowMenu();
                        break;
                    case 4:
                        Console.WriteLine("До свидания!");
                        return;
                    default:
                        ShowError("Введите число от 1 до 4.");
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка: {ex.Message}");
            }
        }
    }

    private void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}

public class Order
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int DriverId { get; set; }
    public int CarId { get; set; }

    public override string ToString()
    {
        return $"Заказ #{Id}: {Date:dd.MM} с {StartTime:hh\\:mm} до {EndTime:hh\\:mm}, " +
               $"Водитель: {DriverId}, Авто: {CarId}";
    }
}

public class OrderManager
{
    private readonly List<Order> _orders = new List<Order>();
    private const string DatePattern = @"^\d{2}\.\d{2}$";
    private const string TimePattern = @"^\d{2}:\d{2}$";

    public void ShowMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("\n--- Управление заказами ---");
            Console.ResetColor();
            Console.WriteLine("1) Добавить заказ");
            Console.WriteLine("2) Просмотреть все заказы");
            Console.WriteLine("3) Вернуться в главное меню");
            Console.Write("Выберите действие: ");

            var input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                ShowError("Некорректный ввод.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    AddOrder();
                    break;
                case 2:
                    ShowAllOrders();
                    break;
                case 3:
                    return;
                default:
                    ShowError("Введите число от 1 до 3.");
                    break;
            }
        }
    }

    private void AddOrder()
    {
        try
        {
            var order = new Order();

            Console.Write("Введите номер заказа: ");
            order.Id = GetValidIntInput("Номер заказа");

            if (_orders.Any(o => o.Id == order.Id))
            {
                ShowError("Заказ с таким номером уже существует.");
                return;
            }

            Console.Write("Введите дату заказа (дд.мм): ");
            order.Date = GetValidDateInput();

            Console.Write("Введите время начала (чч:мм): ");
            order.StartTime = GetValidTimeInput();

            Console.Write("Введите время окончания (чч:мм): ");
            order.EndTime = GetValidTimeInput();

            if (order.EndTime <= order.StartTime)
            {
                ShowError("Время окончания должно быть позже времени начала.");
                return;
            }

            Console.Write("Введите номер водителя: ");
            order.DriverId = GetValidIntInput("Номер водителя");

            Console.Write("Введите номер машины: ");
            order.CarId = GetValidIntInput("Номер машины");

            _orders.Add(order);
            ShowSuccess("Заказ успешно добавлен!");
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка при добавлении заказа: {ex.Message}");
        }
    }

    private DateTime GetValidDateInput()
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (Regex.IsMatch(input, DatePattern))
            {
                try
                {
                    var parts = input.Split('.');
                    var day = int.Parse(parts[0]);
                    var month = int.Parse(parts[1]);
                    var year = DateTime.Now.Year;

                    return new DateTime(year, month, day);
                }
                catch
                {
                    ShowError("Некорректная дата. Попробуйте снова: ");
                }
            }
            else
            {
                ShowError("Формат даты: дд.мм. Попробуйте снова: ");
            }
        }
    }

    private TimeSpan GetValidTimeInput()
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (Regex.IsMatch(input, TimePattern))
            {
                try
                {
                    var parts = input.Split(':');
                    var hours = int.Parse(parts[0]);
                    var minutes = int.Parse(parts[1]);

                    if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59)
                    {
                        throw new ArgumentException();
                    }

                    return new TimeSpan(hours, minutes, 0);
                }
                catch
                {
                    ShowError("Некорректное время. Попробуйте снова: ");
                }
            }
            else
            {
                ShowError("Формат времени: чч:мм. Попробуйте снова: ");
            }
        }
    }

    private void ShowAllOrders()
    {
        if (_orders.Count == 0)
        {
            Console.WriteLine("Заказов нет.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nСписок всех заказов:");
        Console.WriteLine(new string('-', 60));

        foreach (var order in _orders.OrderBy(o => o.Date).ThenBy(o => o.StartTime))
        {
            Console.ResetColor();
            Console.WriteLine(order);
        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\nВсего заказов: {_orders.Count}");
        Console.ResetColor();
    }

    private int GetValidIntInput(string fieldName)
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (int.TryParse(input, out int result) && result > 0)
            {
                return result;
            }
            ShowError($"{fieldName} должен быть положительным числом. Попробуйте снова: ");
        }
    }

    private void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}

public class Driver
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public override string ToString()
    {
        return $"{Id}: {FirstName} {LastName}";
    }
}

public class DriverManager
{
    private readonly List<Driver> _drivers = new List<Driver>();

    public void ShowMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("\n--- Управление водителями ---");
            Console.ResetColor();
            Console.WriteLine("1) Добавить водителя");
            Console.WriteLine("2) Изменить водителя");
            Console.WriteLine("3) Удалить водителя");
            Console.WriteLine("4) Просмотреть всех водителей");
            Console.WriteLine("5) Вернуться в главное меню");
            Console.Write("Выберите действие: ");

            var input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                ShowError("Некорректный ввод.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    AddDriver();
                    break;
                case 2:
                    EditDriver();
                    break;
                case 3:
                    DeleteDriver();
                    break;
                case 4:
                    ShowAllDrivers();
                    break;
                case 5:
                    return;
                default:
                    ShowError("Введите число от 1 до 5.");
                    break;
            }
        }
    }

    private void AddDriver()
    {
        try
        {
            var driver = new Driver();

            Console.Write("Введите ID водителя: ");
            driver.Id = GetValidIntInput("ID водителя");

            if (_drivers.Any(d => d.Id == driver.Id))
            {
                ShowError("Водитель с таким ID уже существует.");
                return;
            }

            Console.Write("Введите имя: ");
            driver.FirstName = GetNonEmptyInput("имя");

            Console.Write("Введите фамилию: ");
            driver.LastName = GetNonEmptyInput("фамилию");

            _drivers.Add(driver);
            ShowSuccess("Водитель успешно добавлен!");
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка при добавлении водителя: {ex.Message}");
        }
    }

    private void EditDriver()
    {
        if (_drivers.Count == 0)
        {
            ShowError("Нет водителей для редактирования.");
            return;
        }

        try
        {
            Console.Write("Введите ID водителя для редактирования: ");
            var id = GetValidIntInput("ID водителя");

            var driver = _drivers.FirstOrDefault(d => d.Id == id);
            if (driver == null)
            {
                ShowError("Водитель с таким ID не найден.");
                return;
            }

            Console.Write("Введите новое имя (Enter - оставить текущее): ");
            var firstName = Console.ReadLine();
            if (!string.IsNullOrEmpty(firstName))
            {
                driver.FirstName = firstName;
            }

            Console.Write("Введите новую фамилию (Enter - оставить текущую): ");
            var lastName = Console.ReadLine();
            if (!string.IsNullOrEmpty(lastName))
            {
                driver.LastName = lastName;
            }

            ShowSuccess("Данные водителя успешно обновлены!");
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка при редактировании водителя: {ex.Message}");
        }
    }

    private void DeleteDriver()
    {
        if (_drivers.Count == 0)
        {
            ShowError("Нет водителей для удаления.");
            return;
        }

        try
        {
            Console.Write("Введите ID водителя для удаления: ");
            var id = GetValidIntInput("ID водителя");

            var driver = _drivers.FirstOrDefault(d => d.Id == id);
            if (driver == null)
            {
                ShowError("Водитель с таким ID не найден.");
                return;
            }

            _drivers.Remove(driver);
            ShowSuccess("Водитель успешно удален!");
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка при удалении водителя: {ex.Message}");
        }
    }

    private void ShowAllDrivers()
    {
        if (_drivers.Count == 0)
        {
            Console.WriteLine("Водителей нет.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nСписок всех водителей:");
        Console.WriteLine(new string('-', 40));

        foreach (var driver in _drivers.OrderBy(d => d.Id))
        {
            Console.ResetColor();
            Console.WriteLine(driver);
        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\nВсего водителей: {_drivers.Count}");
        Console.ResetColor();
    }

    private string GetNonEmptyInput(string fieldName)
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }
            ShowError($"{fieldName} не может быть пустым. Попробуйте снова: ");
        }
    }

    private int GetValidIntInput(string fieldName)
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (int.TryParse(input, out int result) && result > 0)
            {
                return result;
            }
            ShowError($"{fieldName} должен быть положительным числом. Попробуйте снова: ");
        }
    }

    private void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}

public abstract class Vehicle
{
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }

    public abstract string GetVehicleType();
}

public class Sedan : Vehicle
{
    public bool ChildSeatAvailable { get; set; }

    public override string GetVehicleType() => "Легковой автомобиль";

    public override string ToString()
    {
        return $"{Id}: {Brand} {Model} ({Year}), Детское кресло: {(ChildSeatAvailable ? "Да" : "Нет")}";
    }
}

public class Bus : Vehicle
{
    public int Seats { get; set; }

    public override string GetVehicleType() => "Микроавтобус";

    public override string ToString()
    {
        return $"{Id}: {Brand} {Model} ({Year}), Мест: {Seats}";
    }
}

public class CarManager
{
    private readonly List<Vehicle> _vehicles = new List<Vehicle>();

    public void ShowMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("\n--- Управление автомобилями ---");
            Console.ResetColor();
            Console.WriteLine("1) Добавить автомобиль");
            Console.WriteLine("2) Удалить автомобиль");
            Console.WriteLine("3) Просмотреть все автомобили");
            Console.WriteLine("4) Вернуться в главное меню");
            Console.Write("Выберите действие: ");

            var input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                ShowError("Некорректный ввод.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    AddVehicle();
                    break;
                case 2:
                    DeleteVehicle();
                    break;
                case 3:
                    ShowAllVehicles();
                    break;
                case 4:
                    return;
                default:
                    ShowError("Введите число от 1 до 4.");
                    break;
            }
        }
    }

    private void AddVehicle()
    {
        try
        {
            Console.WriteLine("Выберите тип автомобиля:");
            Console.WriteLine("1) Легковой автомобиль");
            Console.WriteLine("2) Микроавтобус");
            Console.Write("Ваш выбор: ");

            var typeChoice = GetValidIntInput("тип автомобиля", 1, 2);
            Vehicle vehicle;

            if (typeChoice == 1)
            {
                var sedan = new Sedan();
                Console.Write("Можно ли с детьми (true/false): ");
                sedan.ChildSeatAvailable = GetValidBoolInput();
                vehicle = sedan;
            }
            else
            {
                var bus = new Bus();
                Console.Write("Введите количество мест: ");
                bus.Seats = GetValidIntInput("количество мест", 1, 50);
                vehicle = bus;
            }

            Console.Write("Введите номер: ");
            vehicle.Id = GetValidIntInput("номер автомобиля");

            if (_vehicles.Any(v => v.Id == vehicle.Id))
            {
                ShowError("Автомобиль с таким номером уже существует.");
                return;
            }

            Console.Write("Введите марку: ");
            vehicle.Brand = GetNonEmptyInput("марку");

            Console.Write("Введите модель: ");
            vehicle.Model = GetNonEmptyInput("модель");

            Console.Write("Введите год выпуска: ");
            vehicle.Year = GetValidIntInput("год выпуска", 1900, DateTime.Now.Year);

            _vehicles.Add(vehicle);
            ShowSuccess("Автомобиль успешно добавлен!");
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка при добавлении автомобиля: {ex.Message}");
        }
    }

    private void DeleteVehicle()
    {
        if (_vehicles.Count == 0)
        {
            ShowError("Нет автомобилей для удаления.");
            return;
        }

        try
        {
            Console.Write("Введите номер автомобиля для удаления: ");
            var id = GetValidIntInput("номер автомобиля");

            var vehicle = _vehicles.FirstOrDefault(v => v.Id == id);
            if (vehicle == null)
            {
                ShowError("Автомобиль с таким номером не найден.");
                return;
            }

            _vehicles.Remove(vehicle);
            ShowSuccess("Автомобиль успешно удален!");
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка при удалении автомобиля: {ex.Message}");
        }
    }

    private void ShowAllVehicles()
    {
        if (_vehicles.Count == 0)
        {
            Console.WriteLine("Автомобилей нет.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nСписок всех автомобилей:");
        Console.WriteLine(new string('-', 60));
        Console.ResetColor();

        var sedans = _vehicles.OfType<Sedan>().ToList();
        var buses = _vehicles.OfType<Bus>().ToList();

        if (sedans.Count > 0)
        {
            Console.WriteLine("\nЛегковые автомобили:");
            foreach (var sedan in sedans.OrderBy(s => s.Id))
            {
                Console.WriteLine(sedan);
            }
        }

        if (buses.Count > 0)
        {
            Console.WriteLine("\nМикроавтобусы:");
            foreach (var bus in buses.OrderBy(b => b.Id))
            {
                Console.WriteLine(bus);
            }
        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\nВсего автомобилей: {_vehicles.Count}");
        Console.WriteLine($"Легковых: {sedans.Count}, Микроавтобусов: {buses.Count}");
        Console.ResetColor();
    }

    private string GetNonEmptyInput(string fieldName)
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }
            ShowError($"{fieldName} не может быть пустым. Попробуйте снова: ");
        }
    }

    private int GetValidIntInput(string fieldName, int min = 1, int max = int.MaxValue)
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (int.TryParse(input, out int result) && result >= min && result <= max)
            {
                return result;
            }
            ShowError($"{fieldName} должен быть числом от {min} до {max}. Попробуйте снова: ");
        }
    }

    private bool GetValidBoolInput()
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (bool.TryParse(input, out bool result))
            {
                return result;
            }
            ShowError("Введите 'true' или 'false'. Попробуйте снова: ");
        }
    }

    private void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
