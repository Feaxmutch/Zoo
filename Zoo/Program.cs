namespace Zoo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Animal> animalTypes = new()
            {
                new Animal("змея", "Ш-ш-ш-ш", Gender.Male),
                new Animal("сова", "ух - хух", Gender.Male),
                new Animal("тигр", "РРРР", Gender.Male),
                new Animal("гусь", "га-га-га", Gender.Male),
            };

            NumberRange animalsCount = new(2, 6);
            ZooFactory zooFactory = new();
            Zoo zoo = zooFactory.Create(animalTypes, animalsCount);
            zoo.Open();
        }
    }

    static class Utilits
    {
        private static Random s_random = new();

        public static Gender GetRandomGender()
        {
            int maleIndex = (int)Gender.Male;
            int femaleIndex = (int)Gender.Female;
            return (Gender)s_random.Next(maleIndex, femaleIndex + 1);
        }
    }

    class AnimalFactory
    {
        private readonly List<Animal> _animalSamples;

        public AnimalFactory(List<Animal> animalSamples)
        {
            ArgumentNullException.ThrowIfNull(animalSamples);

            _animalSamples = animalSamples;
        }

        public List<Animal> Create(int sampleIndex, int count)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
            ArgumentOutOfRangeException.ThrowIfNegative(sampleIndex);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(sampleIndex, _animalSamples.Count);

            List<Animal> animals = new();

            for (int i = 0; i < count; i++)
            {
                animals.Add(_animalSamples[sampleIndex].Clone(true));
            }

            return animals;
        }
    }

    class ZooFactory
    {
        private Random _random;

        public ZooFactory()
        {
            _random = new();
        }

        public Zoo Create(List<Animal> animalTypes, NumberRange animalsCount)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(animalsCount.Minimum);
            ArgumentNullException.ThrowIfNull(animalTypes);

            AnimalFactory animalFactory = new(animalTypes);
            List<Aviary> aviaries = new();

            for (int i = 0; i < animalTypes.Count; i++)
            {
                int count = _random.Next(animalsCount.Minimum, animalsCount.Maximum + 1);
                List<Animal> animals = animalFactory.Create(i, count);
                Aviary aviary = new(animals);
                aviaries.Add(aviary);
            }

            return new Zoo(aviaries);
        }
    }

    class Zoo
    {
        private const string CommandEscape = "escape";

        private readonly List<Aviary> _aviaries;

        public Zoo(List<Aviary> aviaries)
        {
            ArgumentNullException.ThrowIfNull(aviaries);

            _aviaries = aviaries;
        }

        public void Open()
        {
            bool isOpened = true;

            while (isOpened)
            {
                Console.Clear();
                ShowAviarys();
                Console.WriteLine($"Выберите вольер, к которому хотите подойти, или {CommandEscape}, чтобы закрыть программу.");
                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case CommandEscape:
                        isOpened = false;
                        break;

                    default:
                        TrySelectAviary(userInput);
                        break;
                }
            }
        }

        private void ShowAviarys()
        {
            for (int i = 0; i < _aviaries.Count; i++)
            {
                Console.WriteLine($"Вольер {i + 1}");
            }
        }

        private void ShowAnimals(int aviaryNumber)
        {
            Aviary aviary = _aviaries[aviaryNumber - 1];
            Console.WriteLine($"Обитатели вольера {aviaryNumber}");

            foreach (var animal in aviary.Animals)
            {
                Console.Write($"{animal.Name}. Пол - ");

                switch (animal.Gender)
                {
                    case Gender.Male:
                        Console.Write("мужской");
                        break;

                    case Gender.Female:
                        Console.Write("женский");
                        break;
                }

                Console.Write(". Издаёт звук - ");
                animal.MakeSound();
                Console.WriteLine();
            }
        }

        private void TrySelectAviary(string input)
        {
            if (input == string.Empty)
            {
                Console.WriteLine("Вы ничего не ввели");
            }
            else if (int.TryParse(input, out int aviaryNumber) == false)
            {
                Console.WriteLine($"Не получилось конвертировать \"{input}\" в число");
            }
            else
            {
                if (aviaryNumber > 0 && aviaryNumber <= _aviaries.Count)
                {
                    Console.Clear();
                    ShowAnimals(aviaryNumber);
                }
                else
                {
                    Console.WriteLine($"Номера \"{aviaryNumber}\" не существует");
                }
            }

            Console.ReadKey(true);
        }
    }

    class Aviary
    {
        private readonly List<Animal> _animals;

        public Aviary(List<Animal> animals)
        {
            ArgumentNullException.ThrowIfNull(animals);

            _animals = animals;
        }

        public IReadOnlyList<Animal> Animals => _animals;
    }

    class Animal
    {
        private readonly string _sound;

        public Animal(string name, string sound, Gender gender)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(sound);

            Name = name;
            _sound = sound;
            Gender = gender;
        }

        public string Name { get; }

        public Gender Gender { get; }

        public void MakeSound()
        {
            Console.Write(_sound);
        }

        public Animal Clone(bool randomGender = false)
        {
            if (randomGender)
            {
                return new Animal(Name, _sound, Utilits.GetRandomGender());
            }
            else
            {
                return new Animal(Name, _sound, Gender);
            }
        }
    }

    public struct NumberRange
    {
        public NumberRange(int minimum, int maximum)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(maximum, minimum);

            Minimum = minimum;
            Maximum = maximum;
        }

        public int Minimum { get; }

        public int Maximum { get; }
    }

    enum Gender
    {
        Male,
        Female
    }
}
