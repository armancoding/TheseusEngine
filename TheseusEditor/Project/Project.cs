﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.IO;
using TheseusEditor.Utilities;

namespace TheseusEditor.Project
{
    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {
        public static string Extension { get; } = ".theseus";
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Path { get; private set; }

        public string FullName => $"{Path}{Name}{Extension}";

        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes
        { get; private set; }

        private Scene _activeScene;
        public Scene ActiveScene
        { get  => _activeScene;
          set
            {
                if(_activeScene != value)
                {
                    _activeScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
        }

        public static Project Current => Application.Current.MainWindow.DataContext as Project;
        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }
        public static void Save(Project project)
        {
            Serializer.ToFile(project, project.FullName);
        }
        public void Unload()
        { }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if(_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);
        }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            OnDeserialized(new StreamingContext());
        }
    }
}
