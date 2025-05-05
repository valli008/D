using D.Compression;
using D.Encryption;

namespace D
{
    public partial class Form1 : Form
    {
        // ���� ������� ��� ����� ���� �����: (��������, �������������)
        private Dictionary<string, (double timeWeight, double compressionWeight)> fileTypeWeights = new Dictionary<string, (double, double)>
        {
            { ".txt", (0.3, 0.7) },      // ����� � ������� ���������
            { ".jpg", (0.7, 0.3) },      // ���������� � ������� ��������
            { ".jpeg", (0.7, 0.3) },
            { ".pdf", (0.5, 0.5) },      // ������
            { ".png", (0.6, 0.4) },      // ����� ����� ����
            { ".docx", (0.4, 0.6) }      // �������, ��� ������� ���������
        };

        public Form1()
        {
            InitializeComponent();
        }

        // ����������� ������ ��������� ���������
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

        // ����� ��� �������� ������� �� �������� ��� ������
        private bool HasWriteAccess(string directoryPath)
        {
            try
            {
                string testFilePath = Path.Combine(directoryPath, "test.txt");

                // �������� �������� � �������� �������� ����
                File.WriteAllText(testFilePath, "test");
                File.Delete(testFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ����� ��� ���������� ���������� ���� �����
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

        // ������� ������ "�������"
        private void button1_Click(object sender, EventArgs e)
        {
            // �������� ���� �� ����� �� ������ �� �����������
            string folderPath = textBox1.Text; // ��������� ��� ����� �� �����
            string password = textBox2.Text;  // ��������� ��� �������� ������

            // ����������, �� ���� ������� �����
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("����� �� ��������. ������ ��������� ����.", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ������� ���� �� ����� ���� ����� ��� ������ ���������� ��������
            string trainingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "results_database.txt");

            try
            {
                // �������� ������� �� ��������
                string directoryPath = Path.GetDirectoryName(trainingFilePath);
                if (!Directory.Exists(directoryPath) || !HasWriteAccess(directoryPath))
                {
                    MessageBox.Show("���� ���� �� ����� � ���������. �������� ����� �������.", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // �������� �� ���� ���� � ���������, ���� �� ����
                if (!File.Exists(trainingFilePath))
                {
                    File.Create(trainingFilePath).Close(); // ��������� � �������� �����
                }

                // �������� �� ����� � �����
                foreach (var filePath in Directory.GetFiles(folderPath))
                {
                    // ���������� ��������� ��� ��� �����
                    string fileHash = ComputeFileHash(filePath);

                    // ����������, �� ���� ��� � � ��� �����
                    if (File.Exists(trainingFilePath) && File.ReadAllText(trainingFilePath).Contains(fileHash))
                    {
                        continue; // ���������� �������, ���� ���� ��� �������������
                    }

                    // ������� ������ ���������� ��� ����
                    var fileInfo = new FileInfo(filePath);
                    string fileExtension = fileInfo.Extension; // ���������� �����
                    long fileSizeBefore = fileInfo.Length;      // ����� �� �������

                    // ������ ��� ����� � ���� ���� �����
                    File.AppendAllText(trainingFilePath, $"#{fileHash}{Environment.NewLine}");

                    // ���������� �� ��������� ��������� ���������� � ���������
                    foreach (var encryption in encryptionAlgorithms)
                    {
                        foreach (var compression in compressionAlgorithms)
                        {
                            // ������� ���� �����
                            byte[] fileData = File.ReadAllBytes(filePath);

                            // ���������: �������� ����������, ���� ���������
                            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                            byte[] encryptedData = encryption.Encrypt(fileData, password);
                            byte[] compressedData = compression.Compress(encryptedData);
                            stopwatch.Stop();

                            // ���������� ����������
                            long fileSizeAfter = compressedData.Length;
                            double compressionRatio = (double)fileSizeAfter / fileSizeBefore;
                            long processingTime = stopwatch.ElapsedMilliseconds;

                            // �������� ���������� � ���� ���� �����
                            string resultEncryptThenCompress = $"{fileExtension}|{fileSizeBefore}|{encryption.Name} -> {compression.Name}|{fileSizeAfter}|{compressionRatio:F2}|{processingTime} ��";
                            File.AppendAllText(trainingFilePath, resultEncryptThenCompress + Environment.NewLine);

                            // ���������: �������� ���������, ���� ����������
                            stopwatch.Restart();
                            byte[] compressedDataFirst = compression.Compress(fileData);
                            byte[] encryptedDataAfterCompression = encryption.Encrypt(compressedDataFirst, password);
                            stopwatch.Stop();

                            // ���������� ����������
                            fileSizeAfter = encryptedDataAfterCompression.Length;
                            compressionRatio = (double)fileSizeAfter / fileSizeBefore;
                            processingTime = stopwatch.ElapsedMilliseconds;

                            // �������� ���������� � ���� ���� �����
                            string resultCompressThenEncrypt = $"{fileExtension}|{fileSizeBefore}|{compression.Name} -> {encryption.Name}|{fileSizeAfter}|{compressionRatio:F2}|{processingTime} ��";
                            File.AppendAllText(trainingFilePath, resultCompressThenEncrypt + Environment.NewLine);
                        }
                    }
                }

                // ����������� ����������� ��� ���������� ������� ��������
                MessageBox.Show("�������� ���������!", "����", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("���� ���� ��� ��������� ��� ������ � ����. �������� ����� �������.", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������� ��� �������: {ex.Message}", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ������� ������ "ϳ��� ���������"
        private void button2_Click(object sender, EventArgs e)
        {
            string filePath = textBox3.Text;  // ������� ���� �� �����

            // ����������, �� ���� ����
            if (!File.Exists(filePath))
            {
                MessageBox.Show("���� �� ��������. ������ ��������� ����.", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // �������� ����� ����� ��� ������
            FileInfo fileInfo = new FileInfo(filePath);
            long selectedFileSize = fileInfo.Length;

            // ������ ���� ����� ���������� ��������
            string trainingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "results_database.txt");
            string[] databaseLines = File.ReadAllLines(trainingFilePath);

            // �������� ������ ������� (��� �� ���������� ���������)
            bool isTimeBased = radioButton2.Checked;  // ���� ������� ���
            bool isCompressionRatioBased = radioButton1.Checked;  // ���� ������� ���������� ���������

            // ����������� ����� ��� �������� ���������
            double bestScore = double.MaxValue;
            string bestCombination = string.Empty;

            // ������� ��� ��� �������
            List<string[]> tableData = new List<string[]>();

            // ���������� ���������� �������� �� ������� �����
            var groupedResults = GroupResultsByFileSize(databaseLines);

            // ��������� ����� ��� ��������� �����
            var relevantGroup = GetRelevantGroup(groupedResults, selectedFileSize);

            // ���������� ������ �������� ��� ����� �����
            double averageProcessingTime = relevantGroup.Average(r => long.Parse(r.Split('|')[5].Replace(" ��", "")));
            double averageCompressionRatio = relevantGroup.Average(r => double.Parse(r.Split('|')[4]));

            // ���������� �� ������ � ������� ���� ����������
            foreach (var line in relevantGroup)
            {
                var parts = line.Split('|');
                string fileExtension = parts[0];
                string encryptionAlgorithm = parts[2];
                string compressionAlgorithm = parts[3];
                double processingTime = long.Parse(parts[5].Replace(" ��", ""));
                double compressionRatio = double.Parse(parts[4]);

                // ������� ����������� ��� ���� ��������� (�������, �������, ��������)
                double timeMembership = GetTimeMembership(processingTime);

                // ������� ����������� ��� ����������� ��������� (�������, �������, �������)
                double compressionMembership = GetCompressionMembership(compressionRatio);

                // ������������� ������� ����� ��� ���������� ����������
                // ��������� ��� �� ����� ���� �����
                string ext = fileExtension.Trim().ToLower();
                var weights = fileTypeWeights.ContainsKey(ext) ? fileTypeWeights[ext] : (0.5, 0.5); // ��������: ������

                // ����������� ������� � ����������� ���
                var (timeWeight, compressionWeight) = weights;
                double score = timeWeight * timeMembership + compressionWeight * compressionMembership;

                // ����� �������� ���������
                if (score < bestScore)
                {
                    bestScore = score;
                    bestCombination = $"{encryptionAlgorithm} -> {compressionAlgorithm}";
                }

                string scoreDisplay = isTimeBased
                    ? $"{processingTime} ��"
                    : $"{compressionRatio:F2}";

                // ������ �� �������
                tableData.Add(new string[]
                {
                    fileExtension,
                    encryptionAlgorithm,
                    compressionAlgorithm,
                    scoreDisplay
                });
            }

            // ��������� ������� ��� ����������� ����������
            DataGridView resultsTable = new DataGridView();
            resultsTable.DataSource = tableData.Select(row => new { FileExtension = row[0], Encryption = row[1], Compression = row[2], Score = row[3] }).ToList();
            resultsTable.Dock = DockStyle.Fill;

            // ��������� ������� � �������� ���������
            Form resultForm = new Form();
            resultForm.Text = "���������� ������";
            resultForm.Controls.Add(resultsTable);

            // ��������� �������� ���������
            Label bestCombinationLabel = new Label
            {
                Text = $"�������� ���������: {bestCombination}",
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter
            };
            resultForm.Controls.Add(bestCombinationLabel);

            resultForm.Show();
        }
                
        // ������� ��� ���������� ���������� �� ������� �����
        private Dictionary<string, List<string>> GroupResultsByFileSize(string[] databaseLines)
        {
            var groupedResults = new Dictionary<string, List<string>>();

            foreach (var line in databaseLines)
            {
                if (line.StartsWith("#")) continue;

                var parts = line.Split('|');
                long fileSize = long.Parse(parts[1]);

                string group = fileSize < 1048576 ? "Small" : fileSize < 10485760 ? "Medium" : "Large"; // ������ � ������

                if (!groupedResults.ContainsKey(group))
                {
                    groupedResults[group] = new List<string>();
                }

                groupedResults[group].Add(line);
            }

            return groupedResults;
        }

        // ������� ��� ������ �������� ����� ���������� �� ����� ������ �����
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

        // ������ ������� ��������� ��� ����
        private double GetTimeMembership(double time)
        {
            if (time <= 10)
                return 1.0;  // �������
            if (time <= 50)
                return (50 - time) / 40.0;  // �������
            return 0.0;  // ��������
        }

        // ������ ������� ��������� ��� ����������� ���������
        private double GetCompressionMembership(double ratio)
        {
            if (ratio >= 0.5)
                return 1.0;  // ������� ����������
            if (ratio >= 0.3)
                return (0.5 - ratio) / 0.2;  // ������� ����������
            return 0.0;  // ������� ����������
        }

        // ������ ������� ��� ����������� ���� � ���������
        private double ApplyFuzzyRules(double timeMembership, double compressionMembership)
        {
            // ������� �����������:
            // ���� ��� ������� � ���������� ��������� �������, �� �������� �� ������� ��� ������
            return Math.Min(timeMembership, compressionMembership); // ̳���� ��� ������
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // ������� ���� �� ����� � ���������� ����
            string filePath = textBox4.Text;

            // ������� �������� �����
            string password = textBox5.Text;

            // ��������: �� ������� ���� �� �����
            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("���� �� ����� �� ���� ���� �������.", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ��������: �� ������� �������� �����
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("�������� ����� �� ���� ���� �������.", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // ������� ��� � �����
                byte[] fileData = File.ReadAllBytes(filePath);
                byte[] resultData = null; // ����� ��� ��������� ����������

                // �������� �������� �������� ���������� (���� ������)
                var encryption = (checkBox1.Checked || checkBox2.Checked)
                    ? encryptionAlgorithms.FirstOrDefault(a => a.Name == comboBox1.SelectedItem?.ToString())
                    : null;

                // �������� �������� �������� ��������� (���� ������)
                var compression = (checkBox3.Checked || checkBox4.Checked)
                    ? compressionAlgorithms.FirstOrDefault(a => a.Name == comboBox2.SelectedItem?.ToString())
                    : null;

                // ��������: �� �������� �������� �������� ����������
                if ((checkBox1.Checked || checkBox2.Checked) && encryption == null)
                {
                    MessageBox.Show("�� �������� ��������� ��������� ����������.", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ��������: �� �������� �������� �������� ���������
                if ((checkBox3.Checked || checkBox4.Checked) && compression == null)
                {
                    MessageBox.Show("�� �������� ��������� ��������� �����������.", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ��������� ������� ��
                if (checkBox1.Checked && checkBox3.Checked) // ���������� + �����������
                {
                    if (label5.Text == "1") // �������� ����������, ���� ���������
                    {
                        resultData = encryption.Encrypt(fileData, password); // ����������
                        resultData = compression.Compress(resultData); // ���������
                    }
                    else // �������� ���������, ���� ����������
                    {
                        resultData = compression.Compress(fileData); // ���������
                        resultData = encryption.Encrypt(resultData, password); // ����������
                    }
                }
                else if (checkBox2.Checked && checkBox4.Checked) // ������������� + ��������������
                {
                    if (label5.Text == "1") // �������� �������������, ���� ��������������
                    {
                        resultData = encryption.Decrypt(fileData, password); // �������������
                        resultData = compression.Decompress(resultData); // ��������������
                    }
                    else // �������� ��������������, ���� �������������
                    {
                        resultData = compression.Decompress(fileData); // ��������������
                        resultData = encryption.Decrypt(resultData, password); // �������������
                    }
                }
                else if (checkBox1.Checked) // ҳ���� ����������
                {
                    resultData = encryption.Encrypt(fileData, password);
                }
                else if (checkBox2.Checked) // ҳ���� �������������
                {
                    resultData = encryption.Decrypt(fileData, password);
                }
                else if (checkBox3.Checked) // ҳ���� ���������
                {
                    resultData = compression.Compress(fileData);
                }
                else if (checkBox4.Checked) // ҳ���� ��������������
                {
                    resultData = compression.Decompress(fileData);
                }

                // ���������� ���������� � ����
                string outputFilePath = Path.Combine(
                    Path.GetDirectoryName(filePath),
                    Path.GetFileNameWithoutExtension(filePath) + "_processed" + Path.GetExtension(filePath)
                );
                File.WriteAllBytes(outputFilePath, resultData);

                // ����������� ��� ������ �������
                MessageBox.Show($"���� ������ ��������� �� ���������: {outputFilePath}", "����", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // ����������� ��� �������
                MessageBox.Show($"������� �������: {ex.Message}", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // ������ �������� ������
            string label5Text = label5.Text;
            string label6Text = label6.Text;

            // ̳����� �������� �� 1 � 2
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
