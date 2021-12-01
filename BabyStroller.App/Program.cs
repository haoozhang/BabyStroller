using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Channels;
using BabyStroller.SDK;

namespace BabyStroller.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var folder = Path.Combine(Environment.CurrentDirectory, "Animals");
            var files = Directory.GetFiles(folder);
            var animalsTypes = new List<Type>();
            foreach (var file in files)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    /*
                    // 1. Using Reflect
                    if (type.GetMethod("Voice") != null)
                    {
                        animalsTypes.Add(type);
                    }*/
                    
                    // 2. Using Interface
                    if (type.GetInterfaces().Contains(typeof(IAnimal)))
                    {
                        var isUnfinished = type.GetCustomAttributes(false)
                            .Any(a => a.GetType() == typeof(UnfinishedAttribute));
                        if (isUnfinished) continue;
                        animalsTypes.Add(type);
                    }
                }
            }
            
            
            
            // User Interface
            while (true)
            {
                Console.WriteLine("========");
                for (int i = 0; i < animalsTypes.Count; i++)
                {
                    Console.WriteLine($"{i+1}: {animalsTypes[i].Name}");
                }
                Console.WriteLine("========");
                Console.WriteLine("Please choose animal:");
                int index = int.Parse(Console.ReadLine());
                if (index > animalsTypes.Count || index < 1)
                {
                    Console.WriteLine("No such animal! Try again:");
                    continue;
                }
                Console.WriteLine("Please choose times:");
                int times = int.Parse(Console.ReadLine());
                var type = animalsTypes[index - 1];
                var method = type.GetMethod("Voice");
                var instance = Activator.CreateInstance(type);
                /*
                // 1. Using Reflect
                method.Invoke(instance, new object[]{times});*/
                
                // 2. Using Interface
                IAnimal animal = instance as IAnimal;
                animal.Voice(times);
            }
        }
    }
}