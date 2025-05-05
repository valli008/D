using D.Compression;
using D.Encryption;
using System.Diagnostics;

namespace D
{
    public partial class Form1 : Form
    {
        // Ваги критеріїв для різних типів файлів: (вагаЧасу, вагаСтиснення)
        private Dictionary<string, (double timeWeight, double compressionWeight)> fileTypeWeights = new Dictionary<string, (double, double)>
        {
            { ".txt", (0.3, 0.7) },      // Текст — головне стиснення
            { ".jpg", (0.7, 0.3) },      // Зображення — головне швидкість
            { ".jpeg", (0.7, 0.3) },
            { ".pdf", (0.5, 0.5) },      // Баланс
            { ".png", (0.6, 0.4) },      // Трохи більше часу
            { ".docx", (0.4, 0.6) }      // Змішаний, але важливе стиснення
        };

        public Form1()
        {
            InitializeComponent();
        }

        // Ініціалізація списків доступних алгоритмів
        private readonly List<IEncryptionAlgorithm> encryptionAlgorithms = new List<IEncryptionAlgorithm>
        {
            new AESEncryption(),
            new DESEncryption(),
            new TripleDESEncryption(),
            new BlowfishEncryption(),
            new RC4Encryption()
        };

        private readonly List<ICompressionAlgorithm> compressionAlgorithms = new List<ICompressionAlgorithm>
        {
            new DeflateCompression(),
            new LZ4Compression(),
            new Bzip2Compression(),
            new LZMACompression(),
            new PPMCompression()
        };

        // Метод для перевірки доступу до директорії для запису
        private bool HasWriteAccess(string directoryPath)
        {
            try
            {
                string testFilePath = Path.Combine(directoryPath, "test.txt");

                // Спробуємо створити і записати тестовий файл
                File.WriteAllText(testFilePath, "test");
                File.Delete(testFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Метод для обчислення унікального хешу файлу
        private string ComputeFileHash(string filePath)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        // Обробка кнопки "Навчити"
        private void training_Click(object sender, EventArgs e)
        {
            // Отримуємо шлях до папки та пароль від користувача
            string folderPath = PathToTheFolder.Text; // Текстбокс для шляху до папки
            string password = PasswordWord1.Text;  // Текстбокс для введення пароля

            // Перевіряємо, чи існує вказана папка
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("Папку не знайдено. Введіть коректний шлях.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Вказуємо шлях до файлу бази даних для запису результатів навчання
            string trainingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "results_database.txt");

            try
            {
                // Перевірка доступу до директорії
                string directoryPath = Path.GetDirectoryName(trainingFilePath);
                if (!Directory.Exists(directoryPath) || !HasWriteAccess(directoryPath))
                {
                    MessageBox.Show("Немає прав на запис у директорію. Перевірте права доступу.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Перевірка чи існує файл і створення, якщо не існує
                if (!File.Exists(trainingFilePath))
                {
                    File.Create(trainingFilePath).Close(); // Створення і закриття файлу
                }

                // Аналізуємо всі файли в папці
                foreach (var filePath in Directory.GetFiles(folderPath))
                {
                    // Обчислюємо унікальний хеш для файлу
                    string fileHash = ComputeFileHash(filePath); 

                    // Перевіряємо, чи файл вже є в базі даних
                    if (File.Exists(trainingFilePath) && File.ReadAllText(trainingFilePath).Contains(fileHash))
                    {
                        continue; // Пропускаємо обробку, якщо файл уже проаналізовано
                    }

                    // Збираємо базову інформацію про файл
                    var fileInfo = new FileInfo(filePath);
                    string fileExtension = fileInfo.Extension; // Розширення файлу
                    long fileSizeBefore = fileInfo.Length;      // Розмір до обробки

                    // Додаємо хеш файлу у файл бази даних
                    File.AppendAllText(trainingFilePath, $"#{fileHash}{Environment.NewLine}");

                    // Перебираємо всі комбінації алгоритмів шифрування і стиснення
                    foreach (var encryption in encryptionAlgorithms)
                    {
                        foreach (var compression in compressionAlgorithms)
                        {
                            // Зчитуємо вміст файлу
                            byte[] fileData = File.ReadAllBytes(filePath);

                            // Ініціалізація stopwatch
                            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                            // Комбінація: спочатку шифрування, потім стиснення
                            byte[] encryptedData = encryption.Encrypt(fileData, password);
                            byte[] compressedData = compression.Compress(encryptedData);
                            stopwatch.Stop(); // Зупинка таймера

                            // Обчислюємо результати
                            long fileSizeAfter = compressedData.Length;
                            double compressionRatio = (double)fileSizeAfter / fileSizeBefore;
                            long processingTime = stopwatch.ElapsedMilliseconds; // Отримуємо час у мілісекундах

                            // Записуємо результати у файл бази даних
                            string resultEncryptThenCompress = $"{fileExtension}|{fileSizeBefore}|{encryption.Name} -> {compression.Name}|{fileSizeAfter}|{compressionRatio:F2}|{processingTime} мс";
                            File.AppendAllText(trainingFilePath, resultEncryptThenCompress + Environment.NewLine);

                            // Комбінація: спочатку стиснення, потім шифрування
                            stopwatch.Restart(); // Перезапускаємо таймер
                            byte[] compressedDataFirst = compression.Compress(fileData);
                            byte[] encryptedDataAfterCompression = encryption.Encrypt(compressedDataFirst, password);
                            stopwatch.Stop(); // Зупинка таймера

                            // Обчислюємо результати
                            fileSizeAfter = encryptedDataAfterCompression.Length;
                            compressionRatio = (double)fileSizeAfter / fileSizeBefore;
                            processingTime = stopwatch.ElapsedMilliseconds; // Отримуємо час у мілісекундах

                            // Записуємо результати у файл бази даних
                            string resultCompressThenEncrypt = $"{fileExtension}|{fileSizeBefore}|{compression.Name} -> {encryption.Name}|{fileSizeAfter}|{compressionRatio:F2}|{processingTime} мс";
                            File.AppendAllText(trainingFilePath, resultCompressThenEncrypt + Environment.NewLine);
                        }
                    }
                }

                // Повідомляємо користувача про завершення процесу навчання
                MessageBox.Show("Навчання завершено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Немає прав для створення або запису в файл. Перевірте права доступу.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при навчанні: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробка кнопки "Підбір алгоритму"
        private void selection_Click(object sender, EventArgs e)
        {
            string filePath = FilePath1.Text;  // Зчитуємо шлях до файлу

            // Перевіряємо, чи існує файл
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Файл не знайдено. Введіть коректний шлях.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Отримуємо розмір файлу для підбору
            FileInfo fileInfo = new FileInfo(filePath);
            long selectedFileSize = fileInfo.Length;

            // Читаємо базу даних результатів навчання
            string trainingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "results_database.txt");
            string[] databaseLines = File.ReadAllLines(trainingFilePath);

            // Перевірка вибору критерію (час чи коефіцієнт стиснення)
            bool isTimeBased = MinimumExecutionTime.Checked;  // Якщо вибрано час
            bool isCompressionRatioBased = MaximumCompressionRatio.Checked;  // Якщо вибрано коефіцієнт стиснення

            // Ініціалізація змінної для найкращої комбінації
            double bestScore = double.MaxValue;
            string bestCombination = string.Empty;

            // Збираємо дані для таблиці
            List<string[]> tableData = new List<string[]>();

            // Сегментуємо результати навчання за розміром файлів
            var groupedResults = GroupResultsByFileSize(databaseLines);

            // Знаходимо групу для поточного файлу
            var relevantGroup = GetRelevantGroup(groupedResults, selectedFileSize);

            // Обчислюємо середні значення для кожної групи
            double averageProcessingTime = relevantGroup.Average(r => long.Parse(r.Split('|')[5].Replace(" мс", "")));
            double averageCompressionRatio = relevantGroup.Average(r => double.Parse(r.Split('|')[4]));

            // Перебираємо всі записи у вибраній групі результатів
            foreach (var line in relevantGroup)
            {
                var parts = line.Split('|');
                string fileExtension = parts[0];
                string encryptionAlgorithm = parts[2];
                string compressionAlgorithm = parts[3];
                double processingTime = long.Parse(parts[5].Replace(" мс", ""));
                double compressionRatio = double.Parse(parts[4]);

                // Нечітка фазифікація для часу виконання (Швидкий, Середній, Повільний)
                double timeMembership = GetTimeMembership(processingTime);

                // Нечітка фазифікація для коефіцієнта стиснення (Високий, Середній, Низький)
                double compressionMembership = GetCompressionMembership(compressionRatio);

                // Використовуємо нечітку логіку для розрахунку результату
                // Отримання ваг на основі типу файлу
                string ext = fileExtension.Trim().ToLower();
                var weights = fileTypeWeights.ContainsKey(ext) ? fileTypeWeights[ext] : (0.5, 0.5); // Стандарт: баланс

                // Комбінування значень з урахуванням ваг
                var (timeWeight, compressionWeight) = weights;
                double score = timeWeight * timeMembership + compressionWeight * compressionMembership;

                // Пошук найкращої комбінації
                if (score < bestScore)
                {
                    bestScore = score;
                    bestCombination = $"{encryptionAlgorithm} -> {compressionAlgorithm}";
                }

                string scoreDisplay = isTimeBased
                    ? $"{processingTime} мс"
                    : $"{compressionRatio:F2}";

                // Додаємо до таблиці
                tableData.Add(new string[]
                {
                    fileExtension,
                    encryptionAlgorithm,
                    compressionAlgorithm,
                    scoreDisplay
                });
            }

            // Створення таблиці для відображення результатів
            DataGridView resultsTable = new DataGridView();
            resultsTable.DataSource = tableData.Select(row => new { FileExtension = row[0], Encryption = row[1], Compression = row[2], Score = row[3] }).ToList();
            resultsTable.Dock = DockStyle.Fill;

            // Виведення таблиці і найкращої комбінації
            Form resultForm = new Form();
            resultForm.Text = "Результати підбору";
            resultForm.Controls.Add(resultsTable);

            // Виведення найкращої комбінації
            Label bestCombinationLabel = new Label
            {
                Text = $"Найкраща комбінація: {bestCombination}",
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter
            };
            resultForm.Controls.Add(bestCombinationLabel);

            resultForm.Show();
        }
                
        // Функція для групування результатів за розміром файлів
        private Dictionary<string, List<string>> GroupResultsByFileSize(string[] databaseLines)
        {
            var groupedResults = new Dictionary<string, List<string>>();

            foreach (var line in databaseLines)
            {
                if (line.StartsWith("#")) continue;

                var parts = line.Split('|');
                long fileSize = long.Parse(parts[1]);

                string group = fileSize < 1048576 ? "Small" : fileSize < 10485760 ? "Medium" : "Large"; // Розміри в байтах

                if (!groupedResults.ContainsKey(group))
                {
                    groupedResults[group] = new List<string>();
                }

                groupedResults[group].Add(line);
            }

            return groupedResults;
        }

        // Функція для вибору відповідної групи результатів на основі розміру файлу
        private List<string> GetRelevantGroup(Dictionary<string, List<string>> groupedResults, long fileSize)
        {
            if (fileSize < 1048576)
            {
                return groupedResults["Small"];
            }
            else if (fileSize < 10485760)
            {
                return groupedResults["Medium"];
            }
            else
            {
                return groupedResults["Large"];
            }
        }

        // Оцінка ступеня належності для часу
        private double GetTimeMembership(double time)
        {
            if (time <= 10)
                return 1.0;  // Швидкий
            if (time <= 50)
                return (50 - time) / 40.0;  // Середній
            return 0.0;  // Повільний
        }

        // Оцінка ступеня належності для коефіцієнта стиснення
        private double GetCompressionMembership(double ratio)
        {
            if (ratio >= 0.5)
                return 1.0;  // Високий коефіцієнт
            if (ratio >= 0.3)
                return (0.5 - ratio) / 0.2;  // Середній коефіцієнт
            return 0.0;  // Низький коефіцієнт
        }

        // Нечіткі правила для комбінування часу і стиснення
        private double ApplyFuzzyRules(double timeMembership, double compressionMembership)
        {
            // Приклад комбінування:
            // Якщо час швидкий і коефіцієнт стиснення високий, то комбінуємо ці фактори для вибору
            return Math.Min(timeMembership, compressionMembership); // Мінімум для вибору
        }

        private void save_Click(object sender, EventArgs e)
        {
            // Зчитуємо шлях до файлу з текстового поля
            string filePath = FilePath2.Text;

            // Зчитуємо парольну фразу
            string password = PasswordWord2.Text;

            // Перевірка: чи введено шлях до файлу
            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("Шлях до файлу не може бути порожнім.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Перевірка: чи введено парольну фразу
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Парольне слово не може бути порожнім.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Зчитуємо дані з файлу
                byte[] fileData = File.ReadAllBytes(filePath);
                byte[] resultData = null; // Змінна для зберігання результату

                // Отримуємо вибраний алгоритм шифрування (якщо обрано)
                var encryption = (this.encryption.Checked || decoding.Checked)
                    ? encryptionAlgorithms.FirstOrDefault(a => a.Name == shyfruvannya.SelectedItem?.ToString())
                    : null;

                // Отримуємо вибраний алгоритм стиснення (якщо обрано)
                var compression = (archiving.Checked || unzipping.Checked)
                    ? compressionAlgorithms.FirstOrDefault(a => a.Name == arkhivuvannya.SelectedItem?.ToString())
                    : null;

                // Перевірка: чи знайдено вибраний алгоритм шифрування
                if ((this.encryption.Checked || decoding.Checked) && encryption == null)
                {
                    MessageBox.Show("Не знайдено вибраного алгоритму шифрування.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Перевірка: чи знайдено вибраний алгоритм стиснення
                if ((archiving.Checked || unzipping.Checked) && compression == null)
                {
                    MessageBox.Show("Не знайдено вибраного алгоритму архівування.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Виконання обраних дій
                if (this.encryption.Checked && archiving.Checked) // Шифрування + архівування
                {
                    if (label5.Text == "1") // Спочатку шифрування, потім стиснення
                    {
                        resultData = encryption.Encrypt(fileData, password); // Шифрування
                        resultData = compression.Compress(resultData); // Стиснення
                    }
                    else // Спочатку стиснення, потім шифрування
                    {
                        resultData = compression.Compress(fileData); // Стиснення
                        resultData = encryption.Encrypt(resultData, password); // Шифрування
                    }
                }
                else if (decoding.Checked && unzipping.Checked) // Розшифрування + розархівування
                {
                    if (label5.Text == "1") // Спочатку розшифрування, потім розархівування
                    {
                        resultData = encryption.Decrypt(fileData, password); // Розшифрування
                        resultData = compression.Decompress(resultData); // Розархівування
                    }
                    else // Спочатку розархівування, потім розшифрування
                    {
                        resultData = compression.Decompress(fileData); // Розархівування
                        resultData = encryption.Decrypt(resultData, password); // Розшифрування
                    }
                }
                else if (this.encryption.Checked) // Тільки шифрування
                {
                    resultData = encryption.Encrypt(fileData, password);
                }
                else if (decoding.Checked) // Тільки розшифрування
                {
                    resultData = encryption.Decrypt(fileData, password);
                }
                else if (archiving.Checked) // Тільки стиснення
                {
                    resultData = compression.Compress(fileData);
                }
                else if (unzipping.Checked) // Тільки розархівування
                {
                    resultData = compression.Decompress(fileData);
                }

                // Збереження результату у файл
                string outputFilePath = Path.Combine(
                    Path.GetDirectoryName(filePath),
                    Path.GetFileNameWithoutExtension(filePath) + "_processed" + Path.GetExtension(filePath)
                );
                File.WriteAllBytes(outputFilePath, resultData);

                // Повідомлення про успішну обробку
                MessageBox.Show($"Файл успішно оброблено та збережено: {outputFilePath}", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Повідомлення про помилку
                MessageBox.Show($"Сталася помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void change_Click(object sender, EventArgs e)
        {
            // Читаємо значення лейблів
            string label5Text = label5.Text;
            string label6Text = label6.Text;

            // Міняємо значення між 1 і 2
            if (label5Text == "1" && label6Text == "2")
            {
                label5.Text = "2";
                label6.Text = "1";
            }
            else
            {
                label5.Text = "1";
                label6.Text = "2";
            }
        }
    }
}
