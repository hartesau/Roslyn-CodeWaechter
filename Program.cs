// AUTOCONFIG: [1, 5]
#nullable disable
// --- FILE: Program.cs ---

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoslynWaechter
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Application.Run(new MainForm());
        }
    }

    public class ClassAnalysis
    {
        public string Name { get; set; }
        public List<string> Felder { get; set; } = new List<string>();
        public Dictionary<string, List<string>> MethodenUndAufrufe { get; set; } = new Dictionary<string, List<string>>();
    }

    public class SoulElement
    {
        public string Grund_der_Existenz { get; set; } = "";
        public string Auswirkung_bei_Aktivierung { get; set; } = "";
    }

    public class ClassSoul
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SoulElement Klasse { get; set; } = new SoulElement();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, SoulElement> Felder { get; set; } = new Dictionary<string, SoulElement>();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, SoulElement> Methoden { get; set; } = new Dictionary<string, SoulElement>();
    }

    public class DiffSession
    {
        public DateTime Timestamp { get; set; }
        public string RtfContent { get; set; }
    }

    public class MatrixSession
    {
        public DateTime Timestamp { get; set; }
        public string RawResponse { get; set; }
    }

    public class MainForm : Form
    {
        private SplitContainer splitMain, splitCode, splitAST, splitSoul;
        private RichTextBox rtbOldCode, rtbNewCode, rtbLog, rtbSoulView, rtbInceptionPrompt, rtbMatrix;
        private TreeView tvDatabaseOld, tvDatabaseNew;
        private TabControl tabBottomArea;
        private TabPage tabLog, tabDatabase, tabExport, tabSoul, tabConfig, tabInception, tabMatrix;
        private Panel pnlTopBar, pnlLogNav, pnlMatrixNav;
        private Button btnAnalyze, btnShowHistory, btnShowChangelog, btnInfo, btnCompile, btnCenter, btnSnapshot, btnLogPrev, btnLogNext;
        private Button btnMatPrev, btnMatNext;
        private Label lblLogTime, lblMatrixTime;
        private CheckBox chkShiftToLeft, chkAutoExit;
        private TextBox txtTargetPath, txtExePath, txtBatPath, txtInceptionDir, txtProjectName, txtApiKey, txtApiLink, txtApiDelay;
        private ComboBox cmbModel;
        private Button btnLoadModels, btnSendApi, btnScanNuget;
        private System.Windows.Forms.Timer parseTimerOld, parseTimerNew;

        private readonly string AppDir = AppDomain.CurrentDomain.BaseDirectory;
        private string backupFolder, historyFile, changelogFile, manualFile, sessionFile, soulDbFile, guiBoundsFile;

        private List<DiffSession> diffHistory = new List<DiffSession>();
        private int currentDiffIndex = -1;

        private List<MatrixSession> matrixHistory = new List<MatrixSession>();
        private int currentMatrixIndex = -1;

        public MainForm()
        {
            backupFolder = Path.Combine(AppDir, "CodeBackups");
            historyFile = Path.Combine(AppDir, "waechter_history.json");
            changelogFile = Path.Combine(AppDir, "waechter_changelog.txt");
            manualFile = Path.Combine(AppDir, "manual.txt");
            sessionFile = Path.Combine(AppDir, "session_state.json");
            soulDbFile = Path.Combine(AppDir, "soul_database.json");
            guiBoundsFile = Path.Combine(AppDir, "gui_bounds.json");

            if (!Directory.Exists(backupFolder)) Directory.CreateDirectory(backupFolder);
            InitializeComponent();
            SetupLiveParsing();
            EnsureManualExists();
        }

        private void InitializeComponent()
        {
            this.Text = "Roslyn Code-Wächter v41 - Dependency Scanner Edition";
            this.Size = new Size(1400, 950);
            this.MinimumSize = new Size(1000, 700);

            pnlTopBar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(45, 45, 48) };
            btnAnalyze = CreateTopButton("⚡ Analyse", 10, 120, Color.FromArgb(0, 122, 204));
            btnShowHistory = CreateTopButton("📜 Audit", 140, 120, Color.FromArgb(70, 70, 70));
            btnShowChangelog = CreateTopButton("📄 Changelog", 270, 120, Color.FromArgb(85, 85, 85));
            btnInfo = CreateTopButton("ℹ️ Info", 400, 100, Color.FromArgb(100, 80, 20));
            btnCompile = CreateTopButton("🔨 Build & Neustart", 510, 180, Color.FromArgb(160, 40, 40));
            
            chkAutoExit = new CheckBox { Text = "Auto-Exit nach Build", Location = new Point(700, 14), ForeColor = Color.White, AutoSize = true, Checked = false };
            
            btnCenter = CreateTopButton("⚖️ Mitte", 860, 100, Color.FromArgb(85, 85, 85));
            btnSnapshot = CreateTopButton("💾 Snapshot", 970, 120, Color.FromArgb(40, 120, 80));
            
            var btnClearLabor = CreateTopButton("🧹 Clear", 1100, 100, Color.FromArgb(120, 80, 40));
            btnClearLabor.Click += (s, e) => { rtbOldCode.Clear(); rtbNewCode.Clear(); };

            btnCenter.Click += (s, e) => {
                if (splitMain.Height > 50) splitMain.SplitterDistance = splitMain.Height / 2;
                if (splitCode.Width > 50) splitCode.SplitterDistance = splitCode.Width / 2;
                if (splitAST.Width > 50) splitAST.SplitterDistance = splitAST.Width / 2;
                if (splitSoul.Height > 50) splitSoul.SplitterDistance = splitSoul.Height / 2;
            };

            btnAnalyze.Click += BtnAnalyze_Click;
            btnShowHistory.Click += (s, e) => { new LogViewerForm(historyFile).Show(); };
            btnShowChangelog.Click += (s, e) => { new LogViewerForm(changelogFile).Show(); };
            btnInfo.Click += (s, e) => { new LogViewerForm(manualFile, true).Show(); };
            btnCompile.Click += BtnCompile_Click;
            btnSnapshot.Click += BtnSnapshot_Click;

            pnlTopBar.Controls.AddRange(new Control[] { btnAnalyze, btnShowHistory, btnShowChangelog, btnInfo, btnCompile, chkAutoExit, btnCenter, btnSnapshot, btnClearLabor });

            splitMain = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 450 };
            splitCode = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 700 };

            splitCode.Panel1.Controls.Add(CreateCodePanel("Status Quo (Alt)", out rtbOldCode, "Alt"));
            splitCode.Panel2.Controls.Add(CreateCodePanel("Neues Update (Neu)", out rtbNewCode, "Neu"));

            tabBottomArea = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            
            tabLog = new TabPage("🔍 Diff-Protokoll");
            pnlLogNav = new Panel { Dock = DockStyle.Top, Height = 30, BackColor = Color.FromArgb(35, 35, 35) };
            btnLogPrev = new Button { Text = "⬆️ Ältere", Dock = DockStyle.Left, Width = 120, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Color.FromArgb(60, 60, 60) };
            btnLogNext = new Button { Text = "⬇️ Neuere", Dock = DockStyle.Left, Width = 120, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Color.FromArgb(60, 60, 60) };
            lblLogTime = new Label { Text = "Keine Analysen vorhanden.", Dock = DockStyle.Fill, ForeColor = Color.Gold, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnLogPrev.Click += (s, e) => NavigateLog(-1);
            btnLogNext.Click += (s, e) => NavigateLog(1);
            pnlLogNav.Controls.Add(lblLogTime); pnlLogNav.Controls.Add(btnLogNext); pnlLogNav.Controls.Add(btnLogPrev);
            rtbLog = new RichTextBox { Dock = DockStyle.Fill, BackColor = Color.FromArgb(20, 20, 20), ForeColor = Color.Lime, Font = new Font("Consolas", 11), ReadOnly = true, BorderStyle = BorderStyle.None };
            SetupContextMenu(rtbLog);
            tabLog.Controls.Add(rtbLog); tabLog.Controls.Add(pnlLogNav);

            tabDatabase = new TabPage("📦 Architektur & Lexikon");
            splitSoul = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 300 };
            splitAST = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 700 };
            splitAST.Panel1.Controls.Add(CreateAstPanel("AST: Status Quo", out tvDatabaseOld));
            splitAST.Panel2.Controls.Add(CreateAstPanel("AST: Update", out tvDatabaseNew));
            tvDatabaseOld.AfterSelect += TvDatabase_AfterSelect; tvDatabaseNew.AfterSelect += TvDatabase_AfterSelect;
            rtbSoulView = new RichTextBox { Dock = DockStyle.Fill, BackColor = Color.FromArgb(25, 25, 25), ForeColor = Color.Gainsboro, Font = new Font("Segoe UI", 12), ReadOnly = true, BorderStyle = BorderStyle.None };
            SetupContextMenu(rtbSoulView);
            rtbSoulView.Text = "\n\n  Wähle im AST Explorer oben ein Element aus, um dessen Seele hier direkt zu lesen.";
            splitSoul.Panel1.Controls.Add(splitAST); splitSoul.Panel2.Controls.Add(rtbSoulView);
            tabDatabase.Controls.Add(splitSoul);

            tabExport = new TabPage("💾 Export");
            tabExport.BackColor = Color.FromArgb(40, 40, 40);
            var btnExpOld = new Button { Text = "Status Quo AST als JSON exportieren", Size = new Size(350, 40), Location = new Point(30, 30), BackColor = Color.FromArgb(70, 70, 70), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnExpNew = new Button { Text = "Update AST als JSON exportieren", Size = new Size(350, 40), Location = new Point(30, 90), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnExpOld.Click += (s, e) => ExportAst(rtbOldCode.Text, "StatusQuo"); btnExpNew.Click += (s, e) => ExportAst(rtbNewCode.Text, "Update");
            tabExport.Controls.AddRange(new Control[] { btnExpOld, btnExpNew });

            tabSoul = new TabPage("✨ Seele-Motor");
            tabSoul.BackColor = Color.FromArgb(40, 40, 40);
            var lblSoulInfo = new Label { Text = "Die KI-Brücke für Metadaten & Beschreibungen.", ForeColor = Color.LightGray, Location = new Point(30, 20), AutoSize = true };
            var btnPrepareSoul = new Button { Text = "🌀 1. Seele anfordern (Prompt -> Clipboard)", Size = new Size(400, 40), Location = new Point(30, 60), BackColor = Color.FromArgb(100, 80, 20), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnFetchSoul = new Button { Text = "📥 2. Seele integrieren (Fetch from Clipboard)", Size = new Size(400, 40), Location = new Point(30, 120), BackColor = Color.FromArgb(0, 160, 100), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnPrepareSoul.Click += BtnPrepareSoul_Click; btnFetchSoul.Click += BtnFetchSoul_Click;
            tabSoul.Controls.AddRange(new Control[] { lblSoulInfo, btnPrepareSoul, btnFetchSoul });

            tabConfig = new TabPage("⚙️ Einstellungen");
            tabConfig.BackColor = Color.FromArgb(40, 40, 40);
            
            var lblConfig = new Label { Text = "1. Ziel-Datei für den Code-Austausch (Program.cs):", ForeColor = Color.LightGray, Location = new Point(30, 20), AutoSize = true };
            txtTargetPath = new TextBox { Location = new Point(30, 50), Size = new Size(600, 30), ReadOnly = true, BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White, Font = new Font("Segoe UI", 10) };
            var btnSelectTarget = new Button { Text = "📂 Program.cs auswählen", Size = new Size(250, 28), Location = new Point(640, 49), BackColor = Color.FromArgb(70, 70, 70), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSelectTarget.Click += (s, e) => { OpenFileDialog ofd = new OpenFileDialog { Filter = "C# Files (*.cs)|*.cs", Title = "Hauptdatei (Program.cs) auswählen", RestoreDirectory = true }; if (ofd.ShowDialog() == DialogResult.OK) txtTargetPath.Text = ofd.FileName; };

            var lblBat = new Label { Text = "2. Eigenes Build-Skript (.bat) ausführen:", ForeColor = Color.LightGray, Location = new Point(30, 100), AutoSize = true };
            txtBatPath = new TextBox { Location = new Point(30, 130), Size = new Size(600, 30), ReadOnly = true, BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White, Font = new Font("Segoe UI", 10) };
            var btnSelectBat = new Button { Text = "⚙️ .bat auswählen", Size = new Size(250, 28), Location = new Point(640, 129), BackColor = Color.FromArgb(70, 70, 70), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSelectBat.Click += (s, e) => { OpenFileDialog ofd = new OpenFileDialog { Filter = "Batch Files (*.bat)|*.bat", Title = "Build-Skript (.bat) auswählen", RestoreDirectory = true }; if (ofd.ShowDialog() == DialogResult.OK) txtBatPath.Text = ofd.FileName; };

            var lblExe = new Label { Text = "3. Ziel-EXE für den automatischen Neustart:", ForeColor = Color.LightGray, Location = new Point(30, 180), AutoSize = true };
            txtExePath = new TextBox { Location = new Point(30, 210), Size = new Size(600, 30), ReadOnly = true, BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White, Font = new Font("Segoe UI", 10) };
            var btnSelectExe = new Button { Text = "🚀 EXE auswählen", Size = new Size(250, 28), Location = new Point(640, 209), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSelectExe.Click += (s, e) => { OpenFileDialog ofd = new OpenFileDialog { Filter = "Ausführbare Datei (*.exe)|*.exe", Title = "Kompilierte EXE auswählen", RestoreDirectory = true }; if (ofd.ShowDialog() == DialogResult.OK) txtExePath.Text = ofd.FileName; };

            tabConfig.Controls.AddRange(new Control[] { lblConfig, txtTargetPath, btnSelectTarget, lblBat, txtBatPath, btnSelectBat, lblExe, txtExePath, btnSelectExe });

            tabInception = new TabPage("🚀 KI-Inception");
            tabInception.BackColor = Color.FromArgb(40, 40, 40);
            
            var pnlInceptionTop = new Panel { Dock = DockStyle.Top, Height = 220 };
            var lblIncDir = new Label { Text = "1. Arbeitsverzeichnis (Stick/Festplatte):", ForeColor = Color.LightGray, Location = new Point(20, 15), AutoSize = true };
            txtInceptionDir = new TextBox { Location = new Point(20, 35), Size = new Size(400, 30), BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White, Font = new Font("Segoe UI", 10) };
            var btnIncDir = new Button { Text = "📂", Location = new Point(425, 34), Size = new Size(40, 28), BackColor = Color.FromArgb(70, 70, 70), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnIncDir.Click += (s, e) => { FolderBrowserDialog fbd = new FolderBrowserDialog { Description = "Wähle das übergeordnete Verzeichnis für das neue Projekt aus." }; if (fbd.ShowDialog() == DialogResult.OK) txtInceptionDir.Text = fbd.SelectedPath; };
            
            var lblProjName = new Label { Text = "2. Projektname (Ordnername):", ForeColor = Color.LightGray, Location = new Point(500, 15), AutoSize = true };
            txtProjectName = new TextBox { Location = new Point(500, 35), Size = new Size(200, 30), BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White, Font = new Font("Segoe UI", 10) };
            
            var btnBootstrap = new Button { Text = "🌱 Projekt züchten & Wächter klonen", Location = new Point(730, 34), Size = new Size(280, 28), BackColor = Color.FromArgb(160, 40, 100), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnBootstrap.Click += BtnBootstrap_Click;

            var lblApi = new Label { Text = "Google Gemini API-Key:", ForeColor = Color.LightGray, Location = new Point(20, 75), AutoSize = true };
            txtApiKey = new TextBox { Location = new Point(20, 95), Size = new Size(680, 30), BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White, Font = new Font("Segoe UI", 10), UseSystemPasswordChar = true };

            var lblModel = new Label { Text = "Modell:", ForeColor = Color.LightGray, Location = new Point(20, 135), AutoSize = true };
            cmbModel = new ComboBox { Location = new Point(80, 133), Size = new Size(200, 30), BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            btnLoadModels = new Button { Text = "🔄 Modelle laden", Location = new Point(290, 132), Size = new Size(130, 28), BackColor = Color.FromArgb(70, 70, 70), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var lblLink = new Label { Text = "API-Link:", ForeColor = Color.LightGray, Location = new Point(430, 135), AutoSize = true };
            txtApiLink = new TextBox { Location = new Point(490, 133), Size = new Size(450, 30), BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White, Font = new Font("Segoe UI", 10) };

            var lblDelay = new Label { Text = "Pause (Sek):", ForeColor = Color.LightGray, Location = new Point(20, 180), AutoSize = true };
            txtApiDelay = new TextBox { Text = "20", Location = new Point(110, 178), Size = new Size(50, 30), BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White, Font = new Font("Segoe UI", 10) };

            btnLoadModels.Click += async (s, e) => {
                if(string.IsNullOrWhiteSpace(txtApiKey.Text)) { MessageBox.Show("API-Key fehlt!"); return; }
                btnLoadModels.Text = "Lade..."; btnLoadModels.Enabled = false;
                try {
                    string url = $"https://generativelanguage.googleapis.com/v1beta/models?key={txtApiKey.Text.Trim()}";
                    ProcessStartInfo psi = new ProcessStartInfo {
                        FileName = "curl",
                        Arguments = $"-s \"{url}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8
                    };
                    string content = "";
                    using (Process process = Process.Start(psi)) {
                        using (StreamReader reader = process.StandardOutput) { content = await reader.ReadToEndAsync(); }
                    }
                    
                    if(!string.IsNullOrWhiteSpace(content) && content.Trim().StartsWith("{")) {
                        var json = JObject.Parse(content);
                        if(json["error"] != null) {
                            MessageBox.Show($"API Fehler: {json["error"]["message"]}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        } else {
                            cmbModel.Items.Clear();
                            foreach(var m in json["models"]) {
                                if(m["supportedGenerationMethods"] != null && m["supportedGenerationMethods"].ToString().Contains("generateContent")) {
                                    string name = m["name"].ToString().Replace("models/", "");
                                    cmbModel.Items.Add(name);
                                }
                            }
                            if(cmbModel.Items.Count > 0) cmbModel.SelectedIndex = 0;
                            MessageBox.Show($"{cmbModel.Items.Count} generative Modelle geladen!", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    } else { MessageBox.Show($"Unerwartete Antwort vom Curl-Befehl:\n{content}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                } catch(Exception ex) { MessageBox.Show("Fehler beim Laden (Curl): " + ex.Message); }
                btnLoadModels.Text = "🔄 Modelle laden"; btnLoadModels.Enabled = true;
            };

            cmbModel.SelectedIndexChanged += (s, e) => {
                if(cmbModel.SelectedItem != null) {
                    txtApiLink.Text = $"https://generativelanguage.googleapis.com/v1beta/models/{cmbModel.SelectedItem}:generateContent";
                }
            };

            pnlInceptionTop.Controls.AddRange(new Control[] { lblIncDir, txtInceptionDir, btnIncDir, lblProjName, txtProjectName, btnBootstrap, lblApi, txtApiKey, lblModel, cmbModel, btnLoadModels, lblLink, txtApiLink, lblDelay, txtApiDelay });
            
            var pnlInceptionMain = new Panel { Dock = DockStyle.Fill };
            var lblPrompt = new Label { Text = "Dein Prompt an die Maschine:", ForeColor = Color.Gold, Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            rtbInceptionPrompt = new RichTextBox { Dock = DockStyle.Fill, BackColor = Color.FromArgb(20, 20, 20), ForeColor = Color.LightSkyBlue, Font = new Font("Consolas", 12), BorderStyle = BorderStyle.None };
            SetupContextMenu(rtbInceptionPrompt);
            
            // V41: Panel vergrößert für den neuen Button
            var pnlInceptionBottom = new Panel { Dock = DockStyle.Bottom, Height = 90, BackColor = Color.FromArgb(30, 30, 30) };
            
            btnSendApi = new Button { Text = "📡 Senden via API", Location = new Point(20, 10), Size = new Size(250, 30), BackColor = Color.FromArgb(30, 50, 70), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnFallbackCopy = new Button { Text = "📋 1. Prompt kopieren (Fallback)", Location = new Point(290, 10), Size = new Size(250, 30), BackColor = Color.FromArgb(85, 85, 85), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnFallbackPaste = new Button { Text = "📥 2. Code in 'Neues Update' einfügen", Location = new Point(560, 10), Size = new Size(280, 30), BackColor = Color.FromArgb(40, 120, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            
            // V41: Der neue Dependency Scanner Button
            btnScanNuget = new Button { Text = "📦 Auto-NuGet Scanner", Location = new Point(20, 50), Size = new Size(250, 30), BackColor = Color.FromArgb(100, 80, 20), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
btnScanNuget.Click += async (s, e) => {
                string targetProgCs = txtTargetPath.Text;
                if(string.IsNullOrWhiteSpace(txtApiLink.Text) || string.IsNullOrWhiteSpace(txtApiKey.Text)) { MessageBox.Show("API-Link oder Key fehlt!"); return; }
                if(string.IsNullOrWhiteSpace(targetProgCs) || !File.Exists(targetProgCs)) { MessageBox.Show("Bitte wähle zuerst eine gültige Ziel-Program.cs in den Einstellungen aus, damit die Pakete im richtigen Projektordner installiert werden können!", "Projekt fehlt", MessageBoxButtons.OK, MessageBoxIcon.Warning); tabBottomArea.SelectedTab = tabConfig; return; }
                
                string codeToScan = rtbNewCode.Text;
                if (string.IsNullOrWhiteSpace(codeToScan)) codeToScan = rtbOldCode.Text;
                if (string.IsNullOrWhiteSpace(codeToScan)) { MessageBox.Show("Kein Code auf der Werkbank gefunden, den ich scannen könnte!"); return; }

                btnScanNuget.Text = "📦 Scanne & Installiere..."; btnScanNuget.Enabled = false; btnScanNuget.BackColor = Color.FromArgb(160, 40, 40);
                string payloadFile = Path.Combine(AppDir, "nuget_payload.json");

                try {
                    string systemInstruction = "\n\n--- WICHTIGE SYSTEM-DIREKTIVE FÜR DIE KI ---\nDu bist ein reiner Dependency-Analyzer. Analysiere den angehängten C#-Code und ermittle ALLE externen NuGet-Pakete, die für die erfolgreiche Kompilierung fehlen. Antworte AUSSCHLIESSLICH mit einem JSON-Objekt mit zwei Schlüsseln:\n1. \"Nachricht\": Kurze Erklärung an den User.\n2. \"NugetPakete\": Ein JSON-Array mit den exakten NuGet-Paketnamen (z.B. [\"Tesseract\", \"Newtonsoft.Json\"]). Wenn keine externen Pakete nötig sind, gib ein leeres Array [] zurück. SENDE ABSOLUT KEINEN C#-CODE! Nur dieses JSON!";
                    
                    var payload = new { contents = new[] { new { parts = new[] { new { text = systemInstruction + "\n\n--- ZU ANALYSIERENDER CODE ---\n\n" + codeToScan } } } } };
                    string jsonPayload = JsonConvert.SerializeObject(payload);
                    File.WriteAllText(payloadFile, jsonPayload, new UTF8Encoding(false));

                    string url = txtApiLink.Text.Trim() + "?key=" + txtApiKey.Text.Trim();
                    ProcessStartInfo psi = new ProcessStartInfo { FileName = "curl", Arguments = $"-s -X POST -H \"Content-Type: application/json\" -d @\"{payloadFile}\" \"{url}\"", RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true, StandardOutputEncoding = Encoding.UTF8 };
                    
                    string responseText = "";
                    using (Process process = Process.Start(psi)) { using (StreamReader reader = process.StandardOutput) { responseText = await reader.ReadToEndAsync(); } }

                    matrixHistory.Add(new MatrixSession { Timestamp = DateTime.Now, RawResponse = string.IsNullOrWhiteSpace(responseText) ? "[LEERE ANTWORT VOM SERVER]" : responseText });
                    currentMatrixIndex = matrixHistory.Count - 1;
                    UpdateMatrixUI();
                    tabBottomArea.SelectedTab = tabMatrix;

                    if(!string.IsNullOrWhiteSpace(responseText) && responseText.Trim().StartsWith("{")) {
                        var respJson = JObject.Parse(responseText);
                        if (respJson["candidates"] != null && respJson["candidates"].Any()) {
                            string geminiText = respJson["candidates"][0]["content"]["parts"][0]["text"].ToString();
                            if(geminiText.Contains("```json")) { geminiText = geminiText.Substring(geminiText.IndexOf("```json") + 7); if(geminiText.Contains("```")) geminiText = geminiText.Substring(0, geminiText.LastIndexOf("```")); } 
                            else if(geminiText.Contains("```")) { geminiText = geminiText.Substring(geminiText.IndexOf("```") + 3); if(geminiText.Contains("```")) geminiText = geminiText.Substring(0, geminiText.LastIndexOf("```")); }
                            geminiText = geminiText.Trim();

                            try {
                                var aiResponse = JObject.Parse(geminiText);
                                string msgPart = aiResponse["Nachricht"]?.ToString();
                                
                                if (aiResponse["NugetPakete"] != null && aiResponse["NugetPakete"] is JArray nugetArray && nugetArray.Any()) {
                                    string projDir = Path.GetDirectoryName(targetProgCs);
                                    foreach (var pkg in nugetArray) {
                                        string pkgName = pkg.ToString().Trim();
                                        if (!string.IsNullOrWhiteSpace(pkgName)) {
                                            Log($"[NUGET SCANNER] Automatische Installation von: {pkgName}...", Color.Gold);
                                            try {
                                                ProcessStartInfo nugetPsi = new ProcessStartInfo { FileName = "dotnet", Arguments = $"add package {pkgName}", WorkingDirectory = projDir, CreateNoWindow = true, UseShellExecute = false, RedirectStandardOutput = true };
                                                using (Process nugetProc = Process.Start(nugetPsi)) { await nugetProc.StandardOutput.ReadToEndAsync(); Log($"[NUGET SUCCESS] {pkgName} installiert.", Color.Lime); }
                                            } catch (Exception nex) { Log($"[NUGET FEHLER] Fehler bei Installation von {pkgName}: {nex.Message}", Color.Red); }
                                        }
                                    }
                                    MessageBox.Show($"Paket-Analyse und Installation abgeschlossen!\n\nNachricht der KI:\n{msgPart}", "Auto-NuGet Scanner", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                } else {
                                    MessageBox.Show($"Die KI hat keine fehlenden NuGet-Pakete für diesen Code erkannt.\n\nNachricht:\n{msgPart}", "Alles komplett", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            } catch { MessageBox.Show("Fehler beim Parsen der Scanner-Antwort. Schau in den Matrix-Tab für Details.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        }
                    }
                } catch(Exception ex) { Log("[SCANNER FEHLER] " + ex.Message, Color.Red); }
                finally {
                    if (File.Exists(payloadFile)) File.Delete(payloadFile);
                    btnScanNuget.Text = "📦 Auto-NuGet Scanner"; btnScanNuget.Enabled = true; btnScanNuget.BackColor = Color.FromArgb(100, 80, 20);
                }
            };

            btnSendApi.Click += async (s, e) => {
                if(string.IsNullOrWhiteSpace(txtApiLink.Text) || string.IsNullOrWhiteSpace(txtApiKey.Text)) { MessageBox.Show("API-Link oder Key fehlt!"); return; }
                if(string.IsNullOrWhiteSpace(rtbInceptionPrompt.Text)) { MessageBox.Show("Prompt ist leer!"); return; }
                
                int delaySec = 20;
                int.TryParse(txtApiDelay.Text, out delaySec);

                btnSendApi.Text = "📡 Sende & Warte..."; btnSendApi.Enabled = false; btnSendApi.BackColor = Color.FromArgb(160, 40, 40);

                string payloadFile = Path.Combine(AppDir, "inception_payload.json");
                try {
                    bool hasMoreParts = true;
                    bool isFirstRequest = true;
                    
                    string systemInstruction = "\n\n--- WICHTIGE SYSTEM-DIREKTIVE FÜR DIE KI ---\nDu bist ein C#-Entwicklungs-Assistent. Antworte ab sofort AUSSCHLIESSLICH mit einem einzigen, gültigen JSON-Objekt. Dein JSON muss exakt diese VIER Schlüssel enthalten:\n1. \"Nachricht\": Erklärungen, Grüße und Anweisungen für den User.\n2. \"Code\": Der kompilierfähige C#-Code der gesamten Datei (ohne Markdown).\n3. \"HatMehrTeile\": Setze dies auf true, wenn der Code zu lang für die maximale Antwortlänge ist und du ihn abbrechen musst. Setze es auf false, wenn der Code komplett gesendet wurde.\n4. \"NugetPakete\": Ein JSON-Array von Strings mit exakten NuGet-Paketnamen, die für diesen Code zwingend zusätzlich benötigt werden (z.B. [\"Tesseract\"]), oder ein leeres Array [], wenn keine externen Bibliotheken gebraucht werden. Sende absolut nichts anderes als dieses JSON zurück!";
                    
                    string currentPrompt = rtbInceptionPrompt.Text + systemInstruction;
                    
                    string contextCodeForApi = rtbNewCode.Text;
                    if (string.IsNullOrWhiteSpace(contextCodeForApi)) contextCodeForApi = rtbOldCode.Text; 
                    
                    rtbNewCode.Clear(); 

                    while (hasMoreParts) {
                        string fullPrompt = currentPrompt;
                        if (isFirstRequest && !string.IsNullOrWhiteSpace(contextCodeForApi)) {
                            fullPrompt += "\n\n--- AKTUELLER QUELLCODE ALS KONTEXT ---\n\n" + contextCodeForApi;
                        }

                        var payload = new { contents = new[] { new { parts = new[] { new { text = fullPrompt } } } } };
                        string jsonPayload = JsonConvert.SerializeObject(payload);
                        File.WriteAllText(payloadFile, jsonPayload, new UTF8Encoding(false));

                        string url = txtApiLink.Text.Trim() + "?key=" + txtApiKey.Text.Trim();
                        
                        ProcessStartInfo psi = new ProcessStartInfo {
                            FileName = "curl",
                            Arguments = $"-s -X POST -H \"Content-Type: application/json\" -d @\"{payloadFile}\" \"{url}\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            StandardOutputEncoding = Encoding.UTF8
                        };
                        
                        string responseText = "";
                        using (Process process = Process.Start(psi)) {
                            using (StreamReader reader = process.StandardOutput) { responseText = await reader.ReadToEndAsync(); }
                        }

                        matrixHistory.Add(new MatrixSession { Timestamp = DateTime.Now, RawResponse = string.IsNullOrWhiteSpace(responseText) ? "[LEERE ANTWORT VOM SERVER]" : responseText });
                        currentMatrixIndex = matrixHistory.Count - 1;
                        UpdateMatrixUI();
                        if (isFirstRequest) tabBottomArea.SelectedTab = tabMatrix;

                        hasMoreParts = false; 

                        if(!string.IsNullOrWhiteSpace(responseText) && responseText.Trim().StartsWith("{")) {
                            var respJson = JObject.Parse(responseText);
                            if (respJson["candidates"] != null && respJson["candidates"].Any()) {
                                string geminiText = respJson["candidates"][0]["content"]["parts"][0]["text"].ToString();
                                
                                if(geminiText.Contains("```json")) {
                                    geminiText = geminiText.Substring(geminiText.IndexOf("```json") + 7);
                                    if(geminiText.Contains("```")) geminiText = geminiText.Substring(0, geminiText.LastIndexOf("```"));
                                } else if(geminiText.Contains("```")) {
                                    geminiText = geminiText.Substring(geminiText.IndexOf("```") + 3);
                                    if(geminiText.Contains("```")) geminiText = geminiText.Substring(0, geminiText.LastIndexOf("```"));
                                }
                                geminiText = geminiText.Trim();

                                try {
                                    var aiResponse = JObject.Parse(geminiText);
                                    string codePart = aiResponse["Code"]?.ToString();
                                    string msgPart = aiResponse["Nachricht"]?.ToString();
                                    
                                    if (aiResponse["HatMehrTeile"] != null) {
                                        hasMoreParts = aiResponse["HatMehrTeile"].Value<bool>();
                                    }

                                    if (!string.IsNullOrWhiteSpace(codePart)) {
                                        rtbNewCode.AppendText(codePart);
                                    }

                                    if (aiResponse["NugetPakete"] != null && aiResponse["NugetPakete"] is JArray nugetArray && nugetArray.Any()) {
                                        string targetProgCs = txtTargetPath.Text;
                                        if (!string.IsNullOrWhiteSpace(targetProgCs) && File.Exists(targetProgCs)) {
                                            string projDir = Path.GetDirectoryName(targetProgCs);
                                            if (!string.IsNullOrWhiteSpace(projDir) && Directory.Exists(projDir)) {
                                                foreach (var pkg in nugetArray) {
                                                    string pkgName = pkg.ToString().Trim();
                                                    if (!string.IsNullOrWhiteSpace(pkgName)) {
                                                        Log($"[NUGET] Automatische Installation von Paket: {pkgName} gestartet...", Color.Gold);
                                                        try {
                                                            ProcessStartInfo nugetPsi = new ProcessStartInfo {
                                                                FileName = "dotnet",
                                                                Arguments = $"add package {pkgName}",
                                                                WorkingDirectory = projDir,
                                                                CreateNoWindow = true,
                                                                UseShellExecute = false,
                                                                RedirectStandardOutput = true
                                                            };
                                                            using (Process nugetProc = Process.Start(nugetPsi)) {
                                                                string nugetOut = await nugetProc.StandardOutput.ReadToEndAsync();
                                                                Log($"[NUGET SUCCESS] {pkgName} erfolgreich im Projekt verankert.", Color.Lime);
                                                            }
                                                        } catch (Exception nex) {
                                                            Log($"[NUGET FEHLER] Fehler bei Installation von {pkgName}: {nex.Message}", Color.Red);
                                                        }
                                                    }
                                                }
                                            }
                                        } else {
                                            Log("[NUGET WARNUNG] Keine gültige Ziel-Program.cs ausgewählt. NuGet-Installation übersprungen.", Color.Orange);
                                        }
                                    }

                                    string tokenStats = "";
                                    if (respJson["usageMetadata"] != null) {
                                        var meta = respJson["usageMetadata"];
                                        string modelVer = respJson["modelVersion"] != null ? respJson["modelVersion"].ToString() : "Gemini Engine";
                                        tokenStats = $"\n\n[MATRIX STATS]\nModell: {modelVer}\nPrompt-Tokens (In): {meta["promptTokenCount"]}\nResponse-Tokens (Out): {meta["candidatesTokenCount"]}\nGesamt-Volumen: {meta["totalTokenCount"]}";
                                        Log($"[API TELEMETRIE] Modell: {modelVer} | In: {meta["promptTokenCount"]} | Out: {meta["candidatesTokenCount"]} | Gesamt: {meta["totalTokenCount"]}", Color.Cyan);
                                    }

                                    if (!hasMoreParts && !string.IsNullOrWhiteSpace(msgPart)) {
                                        string finalMsg = msgPart + tokenStats;
                                        MessageBox.Show(finalMsg, "Nachricht aus der Matrix (v41)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        Log("\n[KI NACHRICHT]\n" + finalMsg + "\n", Color.LightSkyBlue);
                                    }
                                } catch {
                                    rtbNewCode.AppendText(geminiText);
                                    MessageBox.Show("Die KI hat ungültiges JSON gesendet. Roher Text eingefügt. Ping-Pong abgebrochen.", "Warnung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    break;
                                }
                            }
                        } else {
                            MessageBox.Show("Keine gültige JSON-Antwort von der API. Ping-Pong gestoppt.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }

                        if (hasMoreParts) {
                            matrixHistory.Add(new MatrixSession { Timestamp = DateTime.Now, RawResponse = $"[PING-PONG AKTIV] Warte {delaySec} Sekunden auf den nächsten Teil..." });
                            currentMatrixIndex = matrixHistory.Count - 1;
                            UpdateMatrixUI();
                            
                            await Task.Delay(delaySec * 1000);
                            
                            currentPrompt = "Teil empfangen. Bitte sende den exakt nächsten Teil deines Codes nahtlos weiter. Achte ZWINGEND darauf, wieder exakt das gleiche JSON-Format mit 'Nachricht', 'Code', 'HatMehrTeile' und 'NugetPakete' zu verwenden!";
                            isFirstRequest = false;
                        }
                    }
                } catch(Exception ex) { 
                    matrixHistory.Add(new MatrixSession { Timestamp = DateTime.Now, RawResponse = "[INTERNER FEHLER]\n" + ex.Message });
                    currentMatrixIndex = matrixHistory.Count - 1;
                    UpdateMatrixUI();
                    tabBottomArea.SelectedTab = tabMatrix;
                }
                finally {
                    if (File.Exists(payloadFile)) File.Delete(payloadFile);
                    btnSendApi.Text = "📡 Senden via API"; btnSendApi.Enabled = true; btnSendApi.BackColor = Color.FromArgb(30, 50, 70);
                }
            };

            btnFallbackCopy.Click += (s, e) => { 
                if (!string.IsNullOrWhiteSpace(rtbInceptionPrompt.Text)) { 
                    string contextCode = rtbNewCode.Text;
                    if (string.IsNullOrWhiteSpace(contextCode)) contextCode = rtbOldCode.Text;
                    string fullPrompt = rtbInceptionPrompt.Text;
                    if (!string.IsNullOrWhiteSpace(contextCode)) fullPrompt += "\n\n--- AKTUELLER QUELLCODE ALS KONTEXT ---\n\n" + contextCode;
                    Clipboard.SetText(fullPrompt); MessageBox.Show("Prompt inkl. Code kopiert!"); 
                } 
            };
            btnFallbackPaste.Click += (s, e) => { if (Clipboard.ContainsText()) { rtbNewCode.Text = Clipboard.GetText(); tabBottomArea.SelectedTab = tabConfig; } };
            
            pnlInceptionBottom.Controls.AddRange(new Control[] { btnSendApi, btnScanNuget, btnFallbackCopy, btnFallbackPaste });
            pnlInceptionMain.Controls.Add(rtbInceptionPrompt); pnlInceptionMain.Controls.Add(lblPrompt); pnlInceptionMain.Controls.Add(pnlInceptionBottom);
            
            tabInception.Controls.Add(pnlInceptionMain); tabInception.Controls.Add(pnlInceptionTop);

            tabMatrix = new TabPage("🌌 Matrix (Raw API)");
            pnlMatrixNav = new Panel { Dock = DockStyle.Top, Height = 30, BackColor = Color.FromArgb(35, 35, 35) };
            btnMatPrev = new Button { Text = "⬆️ Ältere", Dock = DockStyle.Left, Width = 120, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Color.FromArgb(60, 60, 60) };
            btnMatNext = new Button { Text = "⬇️ Neuere", Dock = DockStyle.Left, Width = 120, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Color.FromArgb(60, 60, 60) };
            lblMatrixTime = new Label { Text = "Noch keine API-Daten empfangen.", Dock = DockStyle.Fill, ForeColor = Color.Cyan, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnMatPrev.Click += (s, e) => NavigateMatrix(-1);
            btnMatNext.Click += (s, e) => NavigateMatrix(1);
            pnlMatrixNav.Controls.Add(lblMatrixTime); pnlMatrixNav.Controls.Add(btnMatNext); pnlMatrixNav.Controls.Add(btnMatPrev);
            rtbMatrix = new RichTextBox { Dock = DockStyle.Fill, BackColor = Color.FromArgb(15, 15, 15), ForeColor = Color.MediumSpringGreen, Font = new Font("Consolas", 11), ReadOnly = true, BorderStyle = BorderStyle.None };
            SetupContextMenu(rtbMatrix);
            tabMatrix.Controls.Add(rtbMatrix); tabMatrix.Controls.Add(pnlMatrixNav);

            tabBottomArea.TabPages.AddRange(new TabPage[] { tabLog, tabDatabase, tabExport, tabSoul, tabConfig, tabInception, tabMatrix });
            splitMain.Panel1.Controls.Add(splitCode);
            splitMain.Panel2.Controls.Add(tabBottomArea);
            this.Controls.Add(splitMain);
            this.Controls.Add(pnlTopBar);
        }

        private Button CreateTopButton(string text, int x, int width, Color back) => new Button { Text = text, Size = new Size(width, 30), Location = new Point(x, 10), FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = back, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

        private void SetupContextMenu(RichTextBox rtb) {
            ContextMenuStrip cms = new ContextMenuStrip();
            var cut = new ToolStripMenuItem("Ausschneiden", null, (s, e) => { if (!rtb.ReadOnly) rtb.Cut(); });
            var copy = new ToolStripMenuItem("Kopieren", null, (s, e) => rtb.Copy());
            var paste = new ToolStripMenuItem("Einfügen", null, (s, e) => { if (!rtb.ReadOnly) rtb.Paste(); });
            var sel = new ToolStripMenuItem("Alles markieren", null, (s, e) => rtb.SelectAll());
            cms.Items.AddRange(new ToolStripItem[] { cut, copy, paste, new ToolStripSeparator(), sel });
            rtb.ContextMenuStrip = cms;
        }

        private Panel CreateCodePanel(string title, out RichTextBox rtbOut, string side) {
            var p = new Panel { Dock = DockStyle.Fill };
            var lbl = new Label { Text = title, Dock = DockStyle.Top, ForeColor = Color.White, BackColor = Color.FromArgb(60, 60, 60), Height = 25, TextAlign = ContentAlignment.MiddleCenter };
            var tools = new Panel { Dock = DockStyle.Top, Height = 25, BackColor = Color.FromArgb(40, 40, 40) };
            if (side == "Neu") { chkShiftToLeft = new CheckBox { Text = "Shift-to-Left", Dock = DockStyle.Left, ForeColor = Color.Gray, AutoSize = true }; tools.Controls.Add(chkShiftToLeft); }
            var c = new Button { Text = "❌", Dock = DockStyle.Right, Width = 30, FlatStyle = FlatStyle.Flat, ForeColor = Color.Coral };
            var v = new Button { Text = "📋", Dock = DockStyle.Right, Width = 30, FlatStyle = FlatStyle.Flat, ForeColor = Color.SkyBlue };
            var rtb = new RichTextBox { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.LightGray, Font = new Font("Consolas", 10), WordWrap = false, BorderStyle = BorderStyle.None, AllowDrop = true };
            rtbOut = rtb; SetupContextMenu(rtb);
            c.Click += (s, e) => { rtb.Clear(); };
            if (side == "Neu") v.Click += BtnPasteNew_Click; else v.Click += (s, ev) => PasteCode(rtb, "Alt");
            rtb.DragEnter += Rtb_DragEnter; rtb.DragDrop += (s, ev) => Rtb_DragDrop(s, ev, side);
            tools.Controls.Add(v); tools.Controls.Add(c); p.Controls.Add(tools); p.Controls.Add(lbl); 
            p.Controls.Add(rtb); rtb.BringToFront();
            return p;
        }

        private Panel CreateAstPanel(string title, out TreeView tvOut) {
            var p = new Panel { Dock = DockStyle.Fill };
            var tv = new TreeView { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.Gainsboro, Font = new Font("Consolas", 11), BorderStyle = BorderStyle.None };
            tvOut = tv; p.Controls.Add(tv);
            p.Controls.Add(new Label { Text = title, Dock = DockStyle.Top, ForeColor = Color.White, BackColor = Color.FromArgb(50, 50, 50), Height = 25, TextAlign = ContentAlignment.MiddleCenter });
            tv.BringToFront(); return p;
        }

        private void SetupLiveParsing() {
            parseTimerOld = new System.Windows.Forms.Timer { Interval = 800 }; parseTimerOld.Tick += (s, e) => { parseTimerOld.Stop(); RebuildTree(rtbOldCode.Text, tvDatabaseOld, "Status Quo"); };
            rtbOldCode.TextChanged += (s, e) => { parseTimerOld.Stop(); parseTimerOld.Start(); };
            parseTimerNew = new System.Windows.Forms.Timer { Interval = 800 }; parseTimerNew.Tick += (s, e) => { parseTimerNew.Stop(); RebuildTree(rtbNewCode.Text, tvDatabaseNew, "Update"); };
            rtbNewCode.TextChanged += (s, e) => { parseTimerNew.Stop(); parseTimerNew.Start(); };
        }

        private void WriteHistory(string action, string details) { 
            try { 
                JArray arr = new JArray();
                if (File.Exists(historyFile)) {
                    string content = File.ReadAllText(historyFile);
                    if (!string.IsNullOrWhiteSpace(content)) { try { arr = JArray.Parse(content); } catch { } }
                }
                arr.Add(new JObject { ["Zeitstempel"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ["Aktion"] = action, ["Details"] = details }); 
                File.WriteAllText(historyFile, arr.ToString(Formatting.Indented)); 
            } catch { } 
        }

        private void BackupCode(string code, string side, string src) { 
            if (string.IsNullOrWhiteSpace(code)) return; 
            string f = Path.Combine(backupFolder, $"Backup_{side}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"); 
            File.WriteAllText(f, code); WriteHistory("Backup", f); 
        }
        
        private void PasteCode(RichTextBox t, string s) { if (Clipboard.ContainsText()) { t.Text = Clipboard.GetText(); BackupCode(t.Text, s, "Paste"); } }
        private void BtnPasteNew_Click(object sender, EventArgs e) { if (Clipboard.ContainsText()) { if (chkShiftToLeft.Checked && !string.IsNullOrWhiteSpace(rtbNewCode.Text)) { rtbOldCode.Text = rtbNewCode.Text; BackupCode(rtbOldCode.Text, "Alt", "Shift"); } PasteCode(rtbNewCode, "Neu"); } }
        private void Rtb_DragEnter(object sender, DragEventArgs e) { if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy; }
        private void Rtb_DragDrop(object sender, DragEventArgs e, string side) { string[] f = (string[])e.Data.GetData(DataFormats.FileDrop); if (f?.Length > 0) { try { var r = (RichTextBox)sender; r.Text = File.ReadAllText(f[0]); BackupCode(r.Text, side, f[0]); } catch { } } }

        private void BtnSnapshot_Click(object sender, EventArgs e)
        {
            SaveState();
            string snapName = $"Snapshot_{DateTime.Now:yyyyMMdd_HHmmss}";
            string snapDir = Path.Combine(backupFolder, snapName);
            Directory.CreateDirectory(snapDir);
            string[] filesToBackup = { historyFile, changelogFile, manualFile, sessionFile, soulDbFile, guiBoundsFile };
            int count = 0;
            foreach (var file in filesToBackup) {
                if (File.Exists(file)) { File.Copy(file, Path.Combine(snapDir, Path.GetFileName(file)), true); count++; }
            }
            string currentRtf = rtbLog.Rtf;
            if (!string.IsNullOrWhiteSpace(currentRtf)) { File.WriteAllText(Path.Combine(snapDir, "current_diff_log.rtf"), currentRtf); count++; }
            MessageBox.Show($"Snapshot '{snapName}' erfolgreich erstellt!\n{count} Dateien wurden gesichert.", "Backup abgeschlossen", MessageBoxButtons.OK, MessageBoxIcon.Information);
            WriteHistory("Snapshot", $"Komplettes System-Backup in Ordner {snapName} erstellt.");
        }

        private void BtnBootstrap_Click(object sender, EventArgs e)
        {
            string targetBase = txtInceptionDir.Text; string projName = txtProjectName.Text;
            if (string.IsNullOrWhiteSpace(targetBase) || string.IsNullOrWhiteSpace(projName)) { MessageBox.Show("Bitte Verzeichnis und Projektname angeben!", "Fehlende Daten", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!Directory.Exists(targetBase)) { MessageBox.Show("Das angegebene Arbeitsverzeichnis existiert nicht.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            string fullProjPath = Path.Combine(targetBase, projName);
            if (Directory.Exists(fullProjPath)) {
                var res = MessageBox.Show($"Der Ordner '{projName}' existiert bereits. Soll der Zuchtprozess trotzdem ausgeführt werden? (Dateien könnten überschrieben werden)", "Ordner existiert", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.No) return;
            } else { Directory.CreateDirectory(fullProjPath); }

            string batPath = Path.Combine(fullProjPath, "inception_bootstrap.bat");
            string batContent = $@"@echo off
echo ===================================================
echo ZUCHT-PROZESS LÄUFT: {projName}
echo ===================================================
cd /d ""{fullProjPath}""
echo 1. Generiere WinForms Struktur...
dotnet new winforms -f net8.0 --force
echo 2. Fuege Roslyn Engine hinzu...
dotnet add package Microsoft.CodeAnalysis.CSharp
echo 3. Fuege JSON Support hinzu...
dotnet add package Newtonsoft.Json
echo ===================================================
echo Setup abgeschlossen. Schliesse Fenster...
timeout /t 2 /nobreak > NUL
";
            File.WriteAllText(batPath, batContent);
            Process proc = Process.Start(new ProcessStartInfo { FileName = batPath, WorkingDirectory = fullProjPath, UseShellExecute = true });
            proc.WaitForExit();

            string formCs = Path.Combine(fullProjPath, "Form1.cs"); string formDesCs = Path.Combine(fullProjPath, "Form1.Designer.cs"); string progCs = Path.Combine(fullProjPath, "Program.cs");
            if (File.Exists(formCs)) File.Delete(formCs); if (File.Exists(formDesCs)) File.Delete(formDesCs);
            
            string baseCode = "// AUTOCONFIG: [1, 5]\n#nullable disable\n// --- FILE: Program.cs ---\n\n// Initialized by Roslyn.KI.Inception.compiler\n// Bereit für den Code der Maschine...\n";
            File.WriteAllText(progCs, baseCode);

            string clonePath = Path.Combine(fullProjPath, $"Roslyn.Inception_{projName}.exe");
            File.Copy(Application.ExecutablePath, clonePath, true);

            string autoBatPath = Path.Combine(fullProjPath, "build.bat");
            File.WriteAllText(autoBatPath, "@echo off\ndotnet build -c Release\n");

            string cloneSessionPath = Path.Combine(fullProjPath, "session_state.json");
            string standardExeTarget = Path.Combine(fullProjPath, "bin", "Release", "net8.0-windows", $"{projName}.exe");
            
            string selectedModel = null;
            if (cmbModel.InvokeRequired) { cmbModel.Invoke(new Action(() => selectedModel = cmbModel.SelectedItem?.ToString())); } 
            else { selectedModel = cmbModel.SelectedItem?.ToString(); }

            var initSession = new {
                OldCode = "", NewCode = "", ShiftChecked = false, ActiveTab = 4, 
                TargetPath = progCs, BatPath = autoBatPath, ExePath = standardExeTarget,
                InceptionDir = "", ApiKey = txtApiKey.Text, 
                SelectedModel = selectedModel, ApiLink = txtApiLink.Text,
                DiffHistory = new List<DiffSession>(), CurrentDiffIndex = -1,
                MatrixHistory = new List<MatrixSession>(), CurrentMatrixIndex = -1,
                AutoExitChecked = chkAutoExit.Checked
            };
            File.WriteAllText(cloneSessionPath, JsonConvert.SerializeObject(initSession, Formatting.Indented));
            if (File.Exists(batPath)) File.Delete(batPath);

            MessageBox.Show($"Zucht-Prozess für '{projName}' erfolgreich abgeschlossen!\n\n- Projekt generiert\n- DLLs eingebunden\n- ZERO-CONFIG: Pfade & Build-Skript verdrahtet\n- Wächter erfolgreich geklont.\n\nDu kannst die neue .exe nun in {fullProjPath} starten.", "Inception Erfolgreich", MessageBoxButtons.OK, MessageBoxIcon.Information);
            WriteHistory("Bootstrap", $"Neues Projekt '{projName}' mit Zero-Config gezüchtet.");
        }

        private void BtnCompile_Click(object sender, EventArgs e)
        {
            string targetPath = txtTargetPath.Text; string batPathUser = txtBatPath.Text; string exePath = txtExePath.Text;
            if (string.IsNullOrWhiteSpace(targetPath) || string.IsNullOrWhiteSpace(batPathUser) || !File.Exists(targetPath) || !File.Exists(batPathUser)) {
                MessageBox.Show("Bitte zuerst im Tab '⚙️ Einstellungen' die korrekte Program.cs UND dein Build-Skript (.bat) auswählen!", "Pfade fehlen", MessageBoxButtons.OK, MessageBoxIcon.Warning); tabBottomArea.SelectedTab = tabConfig; return;
            }

            string codeToSave = ""; string sourceName = "";
            if (!string.IsNullOrWhiteSpace(rtbNewCode.Text)) { codeToSave = rtbNewCode.Text; sourceName = "RECHTEN Fenster (Neues Update)"; } 
            else if (!string.IsNullOrWhiteSpace(rtbOldCode.Text)) { codeToSave = rtbOldCode.Text; sourceName = "LINKEN Fenster (Status Quo)"; } 
            else { MessageBox.Show("Beide Code-Fenster sind leer! Es gibt nichts zu kompilieren.", "Abbruch", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            var res = MessageBox.Show($"ACHTUNG:\nDies überschreibt deine lokale Datei\n'{Path.GetFileName(targetPath)}'\nmit dem Code aus dem {sourceName}!\n\nFortfahren und Build starten?", "Überschreiben & Kompilieren", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.No) return; 
            
            File.WriteAllText(targetPath, codeToSave);
            WriteHistory("Compiler", $"Code aus {sourceName} in {Path.GetFileName(targetPath)} überschrieben.");
            
            rtbOldCode.Text = codeToSave;
            
            SaveState();
            
            string projDir = Path.GetDirectoryName(targetPath); string batWrapperPath = Path.Combine(projDir, "waechter_wrapper.bat");
            string exeCommand = ""; string brainTransferCommand = "";

            if (!string.IsNullOrWhiteSpace(exePath)) {
                string exeDir = Path.GetDirectoryName(exePath);
                brainTransferCommand = $"echo.\necho ===================================================\necho GEHIRN-TRANSPLANTATION LÄUFT...\necho Kopiere Datenbanken nach: {Path.GetFileName(exeDir)}\necho ===================================================\ncopy /Y \"{soulDbFile}\" \"{exeDir}\\\" > NUL\ncopy /Y \"{sessionFile}\" \"{exeDir}\\\" > NUL\ncopy /Y \"{historyFile}\" \"{exeDir}\\\" > NUL\ncopy /Y \"{guiBoundsFile}\" \"{exeDir}\\\" > NUL\ncopy /Y \"{changelogFile}\" \"{exeDir}\\\" > NUL\necho Transfer abgeschlossen.\n";
                exeCommand = $"echo.\necho Oeffne Explorer in {Path.GetFileName(exeDir)}...\nexplorer \"{exeDir}\"\necho Starte App neu...\ncd /d \"{exeDir}\"\nstart \"\" \"{exePath}\"\n";
            }

            string batContent = $"@echo off\necho Warte auf das Schliessen des Waechters...\ntimeout /t 2 /nobreak > NUL\necho Starte Meister-Build in neuem Fenster...\nstart /wait \"\" \"{batPathUser}\"\n{brainTransferCommand}{exeCommand}del \"%~f0\"";
            File.WriteAllText(batWrapperPath, batContent);
            Process.Start(new ProcessStartInfo { FileName = batWrapperPath, WorkingDirectory = projDir, UseShellExecute = true });
            
            if (chkAutoExit.Checked) {
                Application.Exit();
            }
        }

        private void NavigateLog(int direction) {
            if (diffHistory.Count == 0) return; currentDiffIndex += direction;
            if (currentDiffIndex < 0) currentDiffIndex = 0; if (currentDiffIndex >= diffHistory.Count) currentDiffIndex = diffHistory.Count - 1;
            UpdateLogUI();
        }

        private void UpdateLogUI() {
            if (diffHistory.Count == 0 || currentDiffIndex < 0) return;
            rtbLog.Rtf = diffHistory[currentDiffIndex].RtfContent;
            lblLogTime.Text = $"Analyse vom: {diffHistory[currentDiffIndex].Timestamp:dd.MM.yyyy HH:mm:ss} ({currentDiffIndex + 1}/{diffHistory.Count})";
            rtbLog.SelectionStart = rtbLog.TextLength; rtbLog.ScrollToCaret();
        }

        private void NavigateMatrix(int direction) {
            if (matrixHistory.Count == 0) return; currentMatrixIndex += direction;
            if (currentMatrixIndex < 0) currentMatrixIndex = 0; if (currentMatrixIndex >= matrixHistory.Count) currentMatrixIndex = matrixHistory.Count - 1;
            UpdateMatrixUI();
        }

        private void UpdateMatrixUI() {
            if (matrixHistory.Count == 0 || currentMatrixIndex < 0) return;
            rtbMatrix.Text = matrixHistory[currentMatrixIndex].RawResponse;
            lblMatrixTime.Text = $"API Call vom: {matrixHistory[currentMatrixIndex].Timestamp:dd.MM.yyyy HH:mm:ss} ({currentMatrixIndex + 1}/{matrixHistory.Count})";
        }

        private void Log(string m, Color c) { 
            rtbLog.SelectionStart = rtbLog.TextLength; rtbLog.SelectionColor = Color.Gray; rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] ");
            rtbLog.SelectionColor = c; rtbLog.AppendText(m + "\n"); rtbLog.ScrollToCaret(); 
        }

        private void BtnAnalyze_Click(object sender, EventArgs e) {
            tabBottomArea.SelectedTab = tabLog; rtbLog.Clear(); 
            if (string.IsNullOrWhiteSpace(rtbNewCode.Text)) { Log("Kein Code im rechten Fenster (Neues Update). Analyse abgebrochen.", Color.Orange); return; }
            Log("Starte statische Deep-Diff Analyse...", Color.Cyan);
            try { 
                var altDict = string.IsNullOrWhiteSpace(rtbOldCode.Text) ? new Dictionary<string, ClassAnalysis>() : AnalysiereCodeStruktur(rtbOldCode.Text);
                var neuDict = AnalysiereCodeStruktur(rtbNewCode.Text);
                VergleicheStrukturen(altDict, neuDict); 
            } catch (Exception ex) { Log(ex.Message, Color.Red); }
            diffHistory.Add(new DiffSession { Timestamp = DateTime.Now, RtfContent = rtbLog.Rtf });
            currentDiffIndex = diffHistory.Count - 1; UpdateLogUI();
        }

        private void TvDatabase_AfterSelect(object sender, TreeViewEventArgs e) {
            if (e.Node == null) return; string nText = e.Node.Text;
            if (nText.StartsWith("📦") || nText.StartsWith("⚙️") || nText.StartsWith("🧩") || nText.StartsWith("🔗")) { ShowSoul(nText, "Struktur-Ordner", "Dies ist ein Gliederungs-Ordner des AST. Bitte klicke auf eine spezifische Klasse, Methode oder ein Feld darunter, um die KI-Seele zu lesen."); return; }
            TreeNode classNode = e.Node; while (classNode != null && !classNode.Text.StartsWith("🏗️")) classNode = classNode.Parent;
            if (classNode == null) { rtbSoulView.Clear(); return; }
            string clsName = classNode.Text.Replace("🏗️", "").Trim(); var db = LoadSoulDatabase();
            if (!db.ContainsKey(clsName)) { ShowSoul(nText, "N/A", "Klasse noch nicht durch KI beseelt."); return; }
            var soul = db[clsName];
            if (e.Node == classNode) { ShowSoul(clsName, soul.Klasse?.Grund_der_Existenz, soul.Klasse?.Auswirkung_bei_Aktivierung); return; }
            if (nText.EndsWith("()")) {
                string mName = nText.Replace("()", "").Replace("⚙️", "").Trim();
                if (soul.Methoden != null && soul.Methoden.ContainsKey(mName)) ShowSoul(mName, soul.Methoden[mName].Grund_der_Existenz, soul.Methoden[mName].Auswirkung_bei_Aktivierung);
                else ShowSoul(mName, "N/A", "Methode noch nicht beseelt.");
            } else {
                TreeNode parent = e.Node.Parent;
                if (parent != null && parent.Text.Contains("🧩 Felder")) {
                    string fName = nText.Split(' ').Last().Trim();
                    if (soul.Felder != null && soul.Felder.ContainsKey(fName)) ShowSoul(fName, soul.Felder[fName].Grund_der_Existenz, soul.Felder[fName].Auswirkung_bei_Aktivierung);
                    else ShowSoul(fName, "N/A", "Feld noch nicht beseelt.");
                } else { ShowSoul(nText, "N/A", "Keine spezifische Seele für dieses Element."); }
            }
        }

        private void ShowSoul(string title, string grund, string auswirkung) {
            rtbSoulView.Clear(); rtbSoulView.SelectionFont = new Font("Segoe UI", 18, FontStyle.Bold); rtbSoulView.SelectionColor = Color.LightSkyBlue;
            rtbSoulView.AppendText($"📖 {title}\n\n"); rtbSoulView.SelectionFont = new Font("Segoe UI", 13, FontStyle.Bold); rtbSoulView.SelectionColor = Color.Gold;
            rtbSoulView.AppendText("Grund der Existenz:\n"); rtbSoulView.SelectionFont = new Font("Segoe UI", 12, FontStyle.Regular); rtbSoulView.SelectionColor = Color.Gainsboro;
            rtbSoulView.AppendText($"{(string.IsNullOrWhiteSpace(grund) ? "N/A" : grund)}\n\n"); rtbSoulView.SelectionFont = new Font("Segoe UI", 13, FontStyle.Bold); rtbSoulView.SelectionColor = Color.LimeGreen;
            rtbSoulView.AppendText("Auswirkung bei Aktivierung:\n"); rtbSoulView.SelectionFont = new Font("Segoe UI", 12, FontStyle.Regular); rtbSoulView.SelectionColor = Color.Gainsboro;
            rtbSoulView.AppendText($"{(string.IsNullOrWhiteSpace(auswirkung) ? "N/A" : auswirkung)}\n");
        }

        private void ExportAst(string code, string suffix) {
            if (string.IsNullOrWhiteSpace(code)) return; var ast = AnalysiereCodeStruktur(code);
            SaveFileDialog sfd = new SaveFileDialog { Filter = "JSON|*.json", FileName = $"AST_Export_{suffix}_{DateTime.Now:yyyyMMdd}.json", RestoreDirectory = true };
            if (sfd.ShowDialog() == DialogResult.OK) File.WriteAllText(sfd.FileName, JsonConvert.SerializeObject(ast, Formatting.Indented));
        }

        private Dictionary<string, ClassSoul> LoadSoulDatabase() {
            if (!File.Exists(soulDbFile)) return new Dictionary<string, ClassSoul>();
            try { return JsonConvert.DeserializeObject<Dictionary<string, ClassSoul>>(File.ReadAllText(soulDbFile)) ?? new Dictionary<string, ClassSoul>(); } 
            catch { return new Dictionary<string, ClassSoul>(); }
        }

        private void BtnPrepareSoul_Click(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(rtbNewCode.Text)) return;
            var astNeu = AnalysiereCodeStruktur(rtbNewCode.Text); var db = LoadSoulDatabase(); var delta = new Dictionary<string, ClassSoul>();
            foreach (var kvp in astNeu) {
                string clsName = kvp.Key; ClassSoul cSoul = new ClassSoul(); cSoul.Klasse = null; bool hasNew = false;
                if (!db.ContainsKey(clsName) || db[clsName].Klasse == null || string.IsNullOrWhiteSpace(db[clsName].Klasse.Grund_der_Existenz)) { cSoul.Klasse = new SoulElement(); hasNew = true; }
                foreach (string f in kvp.Value.Felder) { string fName = f.Split(' ').Last(); if (!db.ContainsKey(clsName) || db[clsName].Felder == null || !db[clsName].Felder.ContainsKey(fName) || string.IsNullOrWhiteSpace(db[clsName].Felder[fName].Grund_der_Existenz)) { if (cSoul.Felder == null) cSoul.Felder = new Dictionary<string, SoulElement>(); cSoul.Felder[fName] = new SoulElement(); hasNew = true; } }
                foreach (string m in kvp.Value.MethodenUndAufrufe.Keys) { if (!db.ContainsKey(clsName) || db[clsName].Methoden == null || !db[clsName].Methoden.ContainsKey(m) || string.IsNullOrWhiteSpace(db[clsName].Methoden[m].Grund_der_Existenz)) { if (cSoul.Methoden == null) cSoul.Methoden = new Dictionary<string, SoulElement>(); cSoul.Methoden[m] = new SoulElement(); hasNew = true; } }
                if (hasNew) { if (cSoul.Felder != null && cSoul.Felder.Count == 0) cSoul.Felder = null; if (cSoul.Methoden != null && cSoul.Methoden.Count == 0) cSoul.Methoden = null; delta[clsName] = cSoul; }
            }
            if (delta.Count == 0) { MessageBox.Show("Alles aktuell!"); return; }
            string prompt = @"Du bist ein professioneller Technical Writer und Software-Erklärer. Deine Aufgabe ist es, das folgende JSON-Delta mit Leben zu füllen. Erkläre für jedes neue Bauteil und jede Methode den 'Grund_der_Existenz' und die 'Auswirkung_bei_Aktivierung' absolut laienverständlich. Halte dich nicht kurz erstelle ausführlich aber präzise und nenne den Nutzen. Nutze den unten angehängten Quellcode als Kontext, um genau zu verstehen, was die Bauteile tun! GIB MIR AUSSCHLIESSLICH DAS FERTIGE JSON ZURÜCK! Kein Markdown, keine Einleitung, nur das rohe JSON: " + JsonConvert.SerializeObject(delta, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) + "\n\n--- VOLLSTÄNDIGER QUELLCODE ALS KONTEXT ---\n\n" + rtbNewCode.Text;
            Clipboard.SetText(prompt); MessageBox.Show("Prompt inkl. vollständigem Quellcode-Kontext im Clipboard!", "Kontext bereit", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnFetchSoul_Click(object sender, EventArgs e) {
            if (!Clipboard.ContainsText()) return; string clip = Clipboard.GetText();
            try {
                if (clip.Contains("```json")) { clip = clip.Substring(clip.IndexOf("```json") + 7); clip = clip.Substring(0, clip.LastIndexOf("```")); }
                var newSouls = JsonConvert.DeserializeObject<Dictionary<string, ClassSoul>>(clip.Trim()); var db = LoadSoulDatabase();
                foreach (var kvp in newSouls) {
                    string cls = kvp.Key; if (!db.ContainsKey(cls)) db[cls] = new ClassSoul();
                    if (kvp.Value.Klasse != null && !string.IsNullOrWhiteSpace(kvp.Value.Klasse.Grund_der_Existenz)) db[cls].Klasse = kvp.Value.Klasse;
                    if (kvp.Value.Felder != null) { if (db[cls].Felder == null) db[cls].Felder = new Dictionary<string, SoulElement>(); foreach (var f in kvp.Value.Felder) db[cls].Felder[f.Key] = f.Value; }
                    if (kvp.Value.Methoden != null) { if (db[cls].Methoden == null) db[cls].Methoden = new Dictionary<string, SoulElement>(); foreach (var m in kvp.Value.Methoden) db[cls].Methoden[m.Key] = m.Value; }
                }
                File.WriteAllText(soulDbFile, JsonConvert.SerializeObject(db, Formatting.Indented)); MessageBox.Show("Seele integriert!");
            } catch { MessageBox.Show("Fehler beim Lesen!"); }
        }

        private void RebuildTree(string code, TreeView tv, string t) {
            tv.Nodes.Clear(); if (string.IsNullOrWhiteSpace(code)) return; var r = new TreeNode("📦 Architektur: " + t) { ForeColor = Color.Gold };
            try { var cu = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot(); foreach (var k in cu.DescendantNodes().OfType<ClassDeclarationSyntax>()) {
                var kn = new TreeNode("🏗️ " + k.Identifier.Text) { ForeColor = Color.SkyBlue };
                var fn = new TreeNode("🧩 Felder"); fn.Nodes.AddRange(k.DescendantNodes().OfType<FieldDeclarationSyntax>().SelectMany(f => f.Declaration.Variables.Select(v => new TreeNode($"{f.Declaration.Type} {v.Identifier.Text}"))).ToArray());
                var mn = new TreeNode("⚙️ Methoden"); foreach (var m in k.DescendantNodes().OfType<MethodDeclarationSyntax>()) {
                    var mnode = new TreeNode(m.Identifier.Text + "()"); var calls = m.DescendantNodes().OfType<InvocationExpressionSyntax>().Select(a => a.Expression.ToString()).Distinct();
                    if (calls.Any()) { var cn = new TreeNode("🔗 Ruft auf"); cn.Nodes.AddRange(calls.Select(c => new TreeNode(c)).ToArray()); mnode.Nodes.Add(cn); } mn.Nodes.Add(mnode);
                }
                if (fn.Nodes.Count > 0) kn.Nodes.Add(fn); if (mn.Nodes.Count > 0) kn.Nodes.Add(mn); r.Nodes.Add(kn);
            } } catch { } tv.Nodes.Add(r); r.ExpandAll();
        }

        private Dictionary<string, ClassAnalysis> AnalysiereCodeStruktur(string c) {
            var res = new Dictionary<string, ClassAnalysis>(); var root = CSharpSyntaxTree.ParseText(c).GetCompilationUnitRoot();
            foreach (var k in root.DescendantNodes().OfType<ClassDeclarationSyntax>()) {
                var a = new ClassAnalysis { Name = k.Identifier.Text };
                a.Felder.AddRange(k.DescendantNodes().OfType<FieldDeclarationSyntax>().SelectMany(f => f.Declaration.Variables.Select(v => $"{f.Declaration.Type} {v.Identifier.Text}")));
                foreach (var m in k.DescendantNodes().OfType<MethodDeclarationSyntax>()) a.MethodenUndAufrufe[m.Identifier.Text] = m.DescendantNodes().OfType<InvocationExpressionSyntax>().Select(i => i.Expression.ToString()).Distinct().ToList();
                res[a.Name] = a;
            } return res;
        }

        private void VergleicheStrukturen(Dictionary<string, ClassAnalysis> alt, Dictionary<string, ClassAnalysis> neu) {
            bool abweichungGefunden = false; StringBuilder sb = new StringBuilder(); sb.AppendLine($"=== DIFF: {DateTime.Now} ==="); Log("\n--- DEEP-VERGLEICHS-PROTOKOLL ---", Color.White);
            if (alt.Count == 0 && neu.Count > 0) { Log("\n[INFO] INITIAL COMMIT: Neues Projekt erkannt!\n", Color.Cyan); abweichungGefunden = true; }
            foreach (var k in alt) {
                if (!neu.ContainsKey(k.Key)) { Log($"ALARM: Klasse {k.Key} GELÖSCHT!", Color.Red); sb.AppendLine("- KLASSE GELÖSCHT: " + k.Key); abweichungGefunden = true; continue; }
                var a = k.Value; var b = neu[k.Key];
                foreach (var f in a.Felder.Except(b.Felder)) { Log("[WARNUNG] ENTFERNT: " + f, Color.Orange); abweichungGefunden = true; }
                foreach (var f in b.Felder.Except(a.Felder)) { Log("[INFO] NEU: " + f, Color.Lime); abweichungGefunden = true; }
                foreach (var m in a.MethodenUndAufrufe.Keys.Except(b.MethodenUndAufrufe.Keys)) { Log("[WARNUNG] ENTFERNT: " + m, Color.Orange); abweichungGefunden = true; }
                foreach (var m in b.MethodenUndAufrufe.Keys.Except(a.MethodenUndAufrufe.Keys)) { Log("[INFO] NEU: " + m + "()", Color.Lime); abweichungGefunden = true; if (b.MethodenUndAufrufe[m].Any()) Log("   -> Verdrahtung: " + string.Join(", ", b.MethodenUndAufrufe[m]), Color.DarkGray); }
            }
            foreach (var k in neu) {
                if (!alt.ContainsKey(k.Key)) { Log($"[INFO] NEUE KLASSE: {k.Key}", Color.Lime); abweichungGefunden = true; foreach (var f in k.Value.Felder) Log($"  + Feld: {f}", Color.LimeGreen); foreach (var m in k.Value.MethodenUndAufrufe.Keys) Log($"  + Methode: {m}()", Color.LimeGreen); }
            }
            if (!abweichungGefunden) Log("\n[OK] Identisch.", Color.Lime); else Log("\n[FEHLER] Abweichung!", Color.Red); File.AppendAllText(changelogFile, sb.ToString());
        }

        private void EnsureManualExists() { if (!File.Exists(manualFile)) File.WriteAllText(manualFile, "ROSLYN CODE-WÄCHTER v41 - Dependency Scanner Edition\nBedienung: Mit 'Snapshot' oben rechts alles als Backup sichern."); }

        private void SaveState() {
            var b = WindowState == FormWindowState.Minimized ? RestoreBounds : Bounds; 
            File.WriteAllText(guiBoundsFile, JsonConvert.SerializeObject(new { b.X, b.Y, b.Width, b.Height, State = (int)WindowState, Splitter = splitCode.SplitterDistance, SplitMain = splitMain.SplitterDistance, SplitSoul = splitSoul.SplitterDistance })); 

            string selectedModel = null;
            if (cmbModel.InvokeRequired) { cmbModel.Invoke(new Action(() => selectedModel = cmbModel.SelectedItem?.ToString())); } 
            else { selectedModel = cmbModel.SelectedItem?.ToString(); }

            File.WriteAllText(sessionFile, JsonConvert.SerializeObject(new { 
                OldCode = rtbOldCode.Text, NewCode = rtbNewCode.Text, ShiftChecked = chkShiftToLeft.Checked, ActiveTab = tabBottomArea.SelectedIndex, 
                TargetPath = txtTargetPath.Text, BatPath = txtBatPath.Text, ExePath = txtExePath.Text,
                InceptionDir = txtInceptionDir.Text, ApiKey = txtApiKey.Text, 
                SelectedModel = selectedModel, ApiLink = txtApiLink.Text,
                DiffHistory = diffHistory, CurrentDiffIndex = currentDiffIndex,
                MatrixHistory = matrixHistory, CurrentMatrixIndex = currentMatrixIndex,
                AutoExitChecked = chkAutoExit.Checked
            }));
        }

        protected override void OnFormClosing(FormClosingEventArgs e) { base.OnFormClosing(e); SaveState(); }
        
        protected override void OnLoad(EventArgs e) { 
            base.OnLoad(e); 
            if (File.Exists(guiBoundsFile)) { 
                try { dynamic s = JsonConvert.DeserializeObject(File.ReadAllText(guiBoundsFile)); if (Screen.AllScreens.Any(scr => scr.Bounds.IntersectsWith(new Rectangle((int)s.X, (int)s.Y, (int)s.Width, (int)s.Height)))) { SetBounds((int)s.X, (int)s.Y, (int)s.Width, (int)s.Height); WindowState = (FormWindowState)s.State; splitCode.SplitterDistance = s.Splitter; splitMain.SplitterDistance = s.SplitMain; if (((JObject)s).ContainsKey("SplitSoul")) splitSoul.SplitterDistance = s.SplitSoul; } } catch { } 
            } 
            if (File.Exists(sessionFile)) {
                try {
                    dynamic sess = JsonConvert.DeserializeObject(File.ReadAllText(sessionFile));
                    rtbOldCode.Text = sess.OldCode; rtbNewCode.Text = sess.NewCode; chkShiftToLeft.Checked = sess.ShiftChecked; tabBottomArea.SelectedIndex = sess.ActiveTab; txtTargetPath.Text = sess.TargetPath;
                    if (((JObject)sess).ContainsKey("BatPath")) txtBatPath.Text = sess.BatPath; if (((JObject)sess).ContainsKey("ExePath")) txtExePath.Text = sess.ExePath;
                    if (((JObject)sess).ContainsKey("InceptionDir")) txtInceptionDir.Text = sess.InceptionDir; if (((JObject)sess).ContainsKey("ApiKey")) txtApiKey.Text = sess.ApiKey;
                    if (((JObject)sess).ContainsKey("SelectedModel") && sess.SelectedModel != null) { cmbModel.Items.Add(sess.SelectedModel.ToString()); cmbModel.SelectedIndex = 0; }
                    if (((JObject)sess).ContainsKey("ApiLink")) txtApiLink.Text = sess.ApiLink;
                    if (((JObject)sess).ContainsKey("DiffHistory")) { diffHistory = sess.DiffHistory.ToObject<List<DiffSession>>(); currentDiffIndex = sess.CurrentDiffIndex; UpdateLogUI(); }
                    if (((JObject)sess).ContainsKey("MatrixHistory")) { matrixHistory = sess.MatrixHistory.ToObject<List<MatrixSession>>(); currentMatrixIndex = sess.CurrentMatrixIndex; UpdateMatrixUI(); }
                    if (((JObject)sess).ContainsKey("AutoExitChecked")) chkAutoExit.Checked = sess.AutoExitChecked;
                } catch { }
            }
        }
    }

    public class LogViewerForm : Form {
        private RichTextBox r; private string p; private bool e;
        public LogViewerForm(string path, bool editable = false) {
            p = path; e = editable; Text = "Viewer: " + Path.GetFileName(path); Size = new Size(900, 700); StartPosition = FormStartPosition.CenterScreen;
            r = new RichTextBox { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.Gainsboro, Font = new Font("Consolas", 11), ReadOnly = !e, BorderStyle = BorderStyle.None };
            if (e) { r.KeyDown += (s, args) => { if (args.Control && args.KeyCode == Keys.S) { File.WriteAllText(p, r.Text); args.SuppressKeyPress = true; } }; }
            Controls.Add(r); 
            if (File.Exists(p)) { try { if (Path.GetExtension(p) == ".json") { foreach (var i in JArray.Parse(File.ReadAllText(p))) { r.SelectionColor = Color.Cyan; r.AppendText($"[{i["Zeitstempel"]}] {i["Aktion"]}\n"); r.SelectionColor = Color.Gray; r.AppendText($"  -> {i["Details"]}\n\n"); } } else r.Text = File.ReadAllText(p); } catch { } }
        }
    }
}