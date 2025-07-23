using Ascon.Pilot.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyIceLibrary.Helper
{
    public class FileManager
    {
        private readonly IObjectModifier _objectModifier;
        private readonly IFileProvider _fileProvider;
        private readonly ObjectLoader _objectLoader;

        public FileManager(IObjectsRepository objectsRepository, IObjectModifier objectModifier, IFileProvider fileProvider)
        {
            _objectLoader = new ObjectLoader(objectsRepository);
            _objectModifier = objectModifier;
            _fileProvider = fileProvider;
        }

        public void DeleteFiles(Guid[] guids, Guid objectGuid)
        {
            foreach (var fileGuid in guids)
            {
                var builder = _objectModifier.EditById(objectGuid);
                builder.RemoveFile(fileGuid);
                _objectModifier.Apply();
            }
        }

        public async void DownloadFiles(Guid[] guids, Guid objectGuid)
        {
            if (guids.Length > 0)
            {
                var files = new List<IFile>();

                var snapshots = (await _objectLoader.Load(objectGuid)).PreviousFileSnapshots;
                var allFiles = new List<IFile>();

                foreach (var snapshot in snapshots)
                {
                    allFiles.AddRange(snapshot.Files);
                }

                foreach (var fileGuid in guids)
                {
                    files.Add(allFiles.SingleOrDefault(n => n.Id == fileGuid));
                }

                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result != System.Windows.Forms.DialogResult.OK) return;

                    foreach (var file in files)
                    {
                        using (var stream = _fileProvider.OpenRead(file))
                        {
                            try
                            {
                                using (FileStream output = new FileStream(Path.Combine(dialog.SelectedPath, file.Name),
                                           FileMode.Create))
                                {
                                    stream.CopyTo(output);
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK,
                                    System.Windows.MessageBoxImage.Error);
                            }
                        }
                    }
                    System.Windows.MessageBox.Show($"Файлы ({guids.Length}) успешно сканы в папку \n {dialog.SelectedPath}");
                }
            }
        }
    }
}
