using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TsmRptLibrary
{
    internal static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> newItems)
        {
            foreach (T item in newItems)
            {
                collection.Add(item);
            }
        }
    }

    internal static class MessageQueueExtensions
    {
        public static bool IsAbleToPop(this MessageQueue thisMessageQueue, TimeSpan? timeSpan = null) => thisMessageQueue.Receive(timeSpan ?? TimeSpan.Zero) == null;

        public static bool IsQueueEmpty(this MessageQueueWrapper thisMessageQueue)
        {
            var queueEnum = thisMessageQueue.GetMessageEnumerator2();
            //TODO: this will probably throw if queue addres doesnt exist
            return !queueEnum.MoveNext();
        }

        public static Message PeekZero(this MessageQueueWrapper thisMessageQueue, TimeSpan? timeSpan = null) => thisMessageQueue.Peek(timeSpan ?? TimeSpan.Zero);

        public static void ReQueue(this MessageQueueWrapper thisMessageQueue, MemoryStream bodyStream, string label)
        {
            var messageQueueTransaction = new MessageQueueTransaction();
            var message = new Message
            {
                Label = label,
                BodyStream = bodyStream
            };

            messageQueueTransaction.Begin();
            thisMessageQueue.Send(message, messageQueueTransaction);
            messageQueueTransaction.Commit();
        }
    }

    internal static class MessageExtensions
    {
        public static T DeserializeToJson<T>(this Message message)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(message.BodyStream);
        }
    }

    internal static class IntExtensions
    {
        public static bool IsBetween(this int inputInt, int lower, int upper, bool inclusive = true) => inclusive ? lower <= inputInt && inputInt <= upper : lower < inputInt && inputInt < upper;
    }

    internal static class DateTimeExtensions
    {
        public static bool IsMin(this DateTime inputDateTime) => inputDateTime == DateTime.MinValue;

        public static DateTime Next2Am(this DateTime inputDateTime)
        {
            var automationDatePlus2Am = inputDateTime.Date.AddHours(2);
            return inputDateTime <= automationDatePlus2Am ? automationDatePlus2Am : automationDatePlus2Am.AddDays(1);
        }
    }

    internal static class StringExtensions
    {
        public static bool Contains(this IEnumerable<string> myArray, string findString)
        {
            return myArray.Any(s => string.Equals(findString, s, StringComparison.OrdinalIgnoreCase));
        }

        public static bool EqualsCaseInsensitive(this string leftSide, string rightSide) => string.Equals(rightSide, leftSide, StringComparison.OrdinalIgnoreCase);

        public static int GetHours(this string inputString, int defaultValue)
        {
            var inputStringSubstring = "";
            int outputString;

            if ((inputString.Length > 2) && inputString.Contains(":"))
            {
                inputStringSubstring = inputString.Substring(0, inputString.IndexOf(':'));
            }

            return int.TryParse(inputStringSubstring, out outputString) ? outputString : defaultValue;
        }

        public static bool IfBoolThenParseElseDefault(this string inputString, bool defaultValue)
        {
            bool myBoolValue;
            var isBool = bool.TryParse(inputString, out myBoolValue);
            return isBool ? myBoolValue : defaultValue;
        }

        public static int IfIntThenParseElseDefault(this string inputString, int defaultValue)
        {
            int parsedInt;
            var isInt = int.TryParse(inputString, out parsedInt);
            return isInt ? parsedInt : defaultValue;
        }
    }

    /*    internal static class deviceRepositoryExtensions
        {
            public static bool AreThereEventAutomationsForPatientGuid(this IdeviceRepository thisdeviceRepository, Guid patientGuid)
            {
                var deviceRepository = thisdeviceRepository;
                return deviceRepository.GetEventAutomationByPatientguid(patientGuid) != null;
            }

            public static long GetDeviceIdForSerialNumber(this IdeviceRepository thisdeviceRepository, string serialNumber)
            {
                var deviceRepository = thisdeviceRepository;
                return deviceRepository.GetDeviceBySerialNumber(serialNumber).Id;
            }

            public static void SaveEventAutomations(this IdeviceRepository thisdeviceRepository, EventAutomation eventAutomation)
            {
                var ea = eventAutomation;
                var deviceRepository = thisdeviceRepository;
                deviceRepository.CreateEventAutomations(ea);
                deviceRepository.SaveChanges();
            }
        }*/

    internal static class GuidExtensions
    {
        public static Guid ToGuid(this Guid? source) => source ?? Guid.Empty;
    }

    internal static class StartupAutomationExtensions
    {
        public static MemoryStream SerializeFromJson<T>(this StartupAutomation message)
        {
            var startupAutomationDataSerializer = new DataContractJsonSerializer(typeof(StartupAutomation));
            var memoryStream = new MemoryStream();
            startupAutomationDataSerializer.WriteObject(memoryStream, message);

            memoryStream.Position = 0;

            return memoryStream;
        }
    }

    internal static class TimedEventEntryExtensions
    {
        public static int HourIncrementor(this int timedStripsPerDay)
        {
            return timedStripsPerDay == 1
                ? 0
                : timedStripsPerDay == 3
                    ? 12
                    : timedStripsPerDay == 5 ? 6 : timedStripsPerDay == 11 ? 4 : timedStripsPerDay == 24 ? 1 : 2;
        }
    }
}