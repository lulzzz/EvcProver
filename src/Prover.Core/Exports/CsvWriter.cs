﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Prover.Core.Exports
{
    public static class CsvWriter
    {
        public static async Task Write<T>(string fileName, Dictionary<T, string> recordsAndRowFormats,
            bool writeHeader = false)
        {
            using (var streamWriter = new StreamWriter(fileName))
            {
                using (streamWriter)
                {
                    
                    foreach (var item in recordsAndRowFormats)
                    {                
                        await streamWriter.WriteLineAsync(GetRow(item.Value, item.Key));
                    }
                }
            }
        }

        public static async Task Write<T>(string fileName, string rowFormat, List<T> recordsList,
            bool writeHeader = false)
        {
            var fs = new StreamWriter(fileName);
            await Write(fs, rowFormat, recordsList, writeHeader);
        }

        public static async Task Write<T>(StreamWriter streamWriter, string rowFormat, List<T> recordsList, bool writeHeader = false)
        {
            using (streamWriter)
            {
                await streamWriter.WriteLineAsync($"//{rowFormat}");

                foreach (var value in recordsList)
                    await streamWriter.WriteLineAsync(GetRow(rowFormat, value));
            }
        }

        private static string GetRow<T>(string rowFormat, T record)
        {
            var recordType = record.GetType();
            var properties = recordType.GetProperties().ToList();

            foreach (var property in properties)
            {
                try
                {
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() ==
                        typeof(Dictionary<,>))
                    {
                        var pattern = $@"\[{property.Name}\d+\]";
                        if (Regex.IsMatch(rowFormat, pattern))
                        {
                            var values = recordType.GetProperty(property.Name)?.GetValue(record);
                            foreach (Match match in Regex.Matches(rowFormat, pattern))
                            {
                                foreach (Capture capture in match.Captures)
                                {
                                    var key = capture.Value.Replace($"[{property.Name}", string.Empty);
                                    key = key.Replace("]", string.Empty);
                                    var k = Int32.Parse(key);
                                    var value = values.GetType().GetProperty("Item").GetValue(values, new object[] { k });
                                    rowFormat = rowFormat.Replace($"{capture.Value}", value?.ToString());
                                }
                            }
                        }
                    }

                    if (rowFormat.Contains($"[{property.Name}]"))
                    {
                        var value = recordType.GetProperty(property.Name)?.GetValue(record)?.ToString();
                        if (value != null)
                            rowFormat = rowFormat.Replace($"[{property.Name}]", value);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return rowFormat;
        }
    }
}