namespace IMJunior
{
    class Program
    {
        static void Main(string[] args)
        {
            OrderForm orderForm = new OrderForm();
            PaymentHandler paymentHandler = new PaymentHandler();
            IPaymentSystemFactory factory = new PaymentSystemFactory();

            try
            {
                string systemName = orderForm.GetValidPaymentSystem(factory);
                IPaymentSystem paymentSystem = factory.Create(systemName);

                paymentSystem.ProcessPayment();
                paymentHandler.ShowPaymentResult(paymentSystem);
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine($"Ошибка: {exception.Message}\nПожалуйста, попробуйте снова.");
            }
        }
    }

    public interface IPaymentSystem
    {
        string SystemName { get; }
        void ProcessPayment();
        void VerifyPayment();
    }

    public interface IPaymentSystemFactory
    {
        IPaymentSystem Create(string systemId);
        IEnumerable<string> GetAvailableSystems();
    }

    public class PaymentSystem : IPaymentSystem
    {
        public PaymentSystem(string name)
        {
            SystemName = name;
        }

        public string SystemName { get; }

        public virtual void ProcessPayment() =>
            Console.WriteLine($"Перевод на страницу {SystemName}...");

        public virtual void VerifyPayment() =>
            Console.WriteLine($"Проверка платежа через {SystemName}...");
    }

    public class PaymentSystemFactory : IPaymentSystemFactory 
    {
        private const string Qiwi = nameof(Qiwi);
        private const string WebMoney = nameof(WebMoney);
        private const string Card = nameof(Card);

        private readonly Dictionary<string, string> _availableSystems = new(StringComparer.OrdinalIgnoreCase)
        {
            [Qiwi] = Qiwi,
            [WebMoney] = WebMoney,
            [Card] = Card
        };

        public IPaymentSystem Create(string systemId)
        {
            if (_availableSystems.TryGetValue(systemId, out string systemName))
                return new PaymentSystem(systemName);

            throw new ArgumentException("Неизвестная платежная система");
        }

        public IEnumerable<string> GetAvailableSystems() =>
            _availableSystems.Values.OrderBy(systemName => systemName);
    }

    public class OrderForm
    {
        public string GetValidPaymentSystem(IPaymentSystemFactory factory)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Мы принимаем: {string.Join(", ", factory.GetAvailableSystems())}\n" +
                    $"Какой системой вы хотите совершить оплату?");

                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input) == false && factory.GetAvailableSystems().Contains(input, StringComparer.OrdinalIgnoreCase))
                    return input;

                Console.WriteLine("Неизвестная платежная система! Попробуйте еще раз.\n" +
                    "Нажмите любую клавишу");
                Console.ReadKey();
            }
        }
    }

    public class PaymentHandler
    {
        public void ShowPaymentResult(IPaymentSystem paymentSystem)
        {
            Console.WriteLine($"Вы оплатили с помощью {paymentSystem.SystemName}");

            paymentSystem.VerifyPayment();

            Console.WriteLine("Оплата прошла успешно!");
        }
    }
}
