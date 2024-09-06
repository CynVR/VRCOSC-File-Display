using NUnit.Framework;
using Valve.VR;
using VRCOSC.Game.Modules;
using VRCOSC.Game.Modules.Avatar;

namespace CynVR
{
    [ModuleTitle("File Display")]
    [ModuleDescription("Display the contents of a file into the chatbox")]
    [ModuleAuthor("CynVR", "https://github.com/CynVR")]
    [ModuleGroup(ModuleType.Integrations)]
    public partial class FileDisplayModule : ChatBoxModule
    {
        private string? filePath;
        private string? fileContent;
        private bool displayUntilEmptied;
        private bool updateOnEmptied;
        private bool isDirty;

        protected override void CreateAttributes() {
            this.CreateSetting(FileReaderSetting.FilePath, "File Path", "The path to the text file", string.Empty, null);
            this.CreateSetting(FileReaderSetting.DisplayUntilEmptied, "Display Until Emptied", "Display the text until the file is emptied", true, null);
            this.CreateSetting(FileReaderSetting.UpdateOnEmpty, "Update on Empty", "Update the displayed text when the file is emptied", false, null);
            this.CreateVariable(FileReaderVariable.FileContent, "File Content", "content");
            this.CreateState(FileReaderState.Default, "Default", this.GetVariableFormat(FileReaderVariable.FileContent) ?? "");
            this.CreateEvent(FileVariableChanged.TextUpdate, "Content Changed", this.GetVariableFormat(FileReaderVariable.FileContent) ?? "", 4f);
        }

        protected override void OnModuleStart() {
            this.ChangeStateTo(FileReaderState.Default);
            this.filePath = GetSetting<string>(FileReaderSetting.FilePath);
            if (this.IsValidFilePath())
                this.Log("File path is valid. Reading file content...");
            else
                this.Log("Invalid file path. Please provide a valid file path in the module settings.");

            displayUntilEmptied = GetSetting<bool>(FileReaderSetting.DisplayUntilEmptied);
            updateOnEmptied = GetSetting<bool>(FileReaderSetting.UpdateOnEmpty);
        }

        [ModuleUpdate(ModuleUpdateMode.ChatBox, true, 2000)]
        private void updateVariables() {
            if (!this.IsValidFilePath()) {
                this.Log("File path is no longer valid, please restart the module");
                return;
            }

            string tmp = this.ReadFile();
            if (fileContent != tmp) {
                if (!string.IsNullOrEmpty(tmp)) this.Log($"File was updated to: '{tmp}'");
                fileContent = tmp;
                isDirty = true;
            }

            if (string.IsNullOrEmpty(fileContent) && !updateOnEmptied) return;

            if (displayUntilEmptied || isDirty) {
                isDirty = false;
                this.TriggerEvent(FileVariableChanged.TextUpdate);
                this.SetVariableValue(FileReaderVariable.FileContent, fileContent, "");
            }
        }

        private bool IsValidFilePath() {
            return !string.IsNullOrEmpty(this.filePath) && File.Exists(this.filePath);
        }

        private string ReadFile() {
            try {
                if (string.IsNullOrEmpty(filePath)) throw new NullReferenceException();
                return File.ReadAllText(filePath);
            } catch (Exception ex) {
                this.Log("Error reading file: " + ex.Message);
                return "";
            }
        }

        private enum FileReaderSetting
        {
            FilePath,
            DisplayUntilEmptied,
            UpdateOnEmpty,
        }

        private enum FileReaderState
        {
            Default,
        }

        private enum FileReaderVariable
        {
            FileContent,
        }

        private enum FileVariableChanged
        {
            TextUpdate,
        }
    }
}
