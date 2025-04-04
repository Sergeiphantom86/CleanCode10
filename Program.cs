namespace IMJunior
{
    class Program
    {
        static void Main(string[] args)
        {
            PaymentSystemRegistry registry = new PaymentSystemRegistry();
            OrderForm orderForm = new OrderForm();
            PaymentHandler paymentHandler = new PaymentHandler();

            try
            {
                string systemName = orderForm.GetValidPaymentSystem(registry.GetAvailableSystems());
                IPaymentSystemFactory factory = registry.GetFactory(systemName);

                paymentHandler.ProcessPayment(factory);
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine($"Ошибка: {exception.Message}");
                Console.WriteLine("Пожалуйста, попробуйте снова.");
            }
        }
    }

    public interface IPaymentSystem
    {
        void ProcessPayment();
        void VerifyPayment();
    }

    public interface IPaymentSystemFactory
    {
        string SystemName { get; }
        IPaymentSystem CreatePaymentSystem();
    }

    public class QiwiPaymentFactory : IPaymentSystemFactory
    {
        public string SystemName => "QIWI";

        public IPaymentSystem CreatePaymentSystem() => 
            new QiwiPaymentSystem();
    }

    public class WebMoneyPaymentFactory : IPaymentSystemFactory
    {
        public string SystemName => "WebMoney";

        public IPaymentSystem CreatePaymentSystem() => 
            new WebMoneyPaymentSystem();
    }

    public class CardPaymentFactory : IPaymentSystemFactory
    {
        public string SystemName => "Card";

        public IPaymentSystem CreatePaymentSystem() => 
            new CardPaymentSystem();
    }

    public class QiwiPaymentSystem : IPaymentSystem
    {
        public void ProcessPayment() => 
            Console.WriteLine("Перевод на страницу QIWI...");
        public void VerifyPayment() => 
            Console.WriteLine("Проверка платежа через QIWI...");
    }

    public class WebMoneyPaymentSystem : IPaymentSystem
    {
        public void ProcessPayment() => 
            Console.WriteLine("Вызов API WebMoney...");
        public void VerifyPayment() => 
            Console.WriteLine("Проверка платежа через WebMoney...");
    }

    public class CardPaymentSystem : IPaymentSystem
    {
        public void ProcessPayment() => 
            Console.WriteLine("Вызов API банка эмитера карты...");
        public void VerifyPayment() => 
            Console.WriteLine("Проверка платежа через банк...");
    }

    public class PaymentSystemRegistry
    {
        private readonly List<IPaymentSystemFactory> _factories = new()
    {
        new QiwiPaymentFactory(),
        new WebMoneyPaymentFactory(),
        new CardPaymentFactory()
    };

        public IEnumerable<string> GetAvailableSystems() =>
            _factories.Select(factory => factory.SystemName);

        public IPaymentSystemFactory GetFactory(string systemName) =>
            _factories.FirstOrDefault(factory => factory.SystemName.Equals(systemName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Неизвестная платёжная система");
    }

    public class OrderForm
    {
        public string GetValidPaymentSystem(IEnumerable<string> availableSystems)
        {
            List<string> systems = availableSystems.ToList();
            string selectedSystem = null;
            bool isValidInput = false;

            while (isValidInput == false)
            {
                Console.Clear();
                Console.WriteLine($"Мы принимаем: {string.Join(", ", systems)}\n" +
                                "Какой системой вы хотите совершить оплату?");

                string input = Console.ReadLine()?.Trim() ?? "";

                isValidInput = systems.Contains(input, StringComparer.OrdinalIgnoreCase);

                selectedSystem = isValidInput ? input : null;

                if (isValidInput == false)
                {
                    Console.WriteLine("Неизвестная платежная система! Попробуйте еще раз.\n" +
                                    "Нажмите любую клавишу");
                    Console.ReadKey();
                }
            }

            return selectedSystem;
        }
    }

    public class PaymentHandler
    {
        public void ProcessPayment(IPaymentSystemFactory factory)
        {
            IPaymentSystem paymentSystem = factory.CreatePaymentSystem();
            paymentSystem.ProcessPayment();
            ShowPaymentResult(paymentSystem);
        }

        private void ShowPaymentResult(IPaymentSystem paymentSystem)
        {
            paymentSystem.VerifyPayment();
            Console.WriteLine("Оплата прошла успешно!");
        }
    }
}
