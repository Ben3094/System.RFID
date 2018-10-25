using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace System.RFID
{
    public static class GlobalTagCache
    {
        public static Dictionary<Type, Type[]> AvailableTagTypes = new Dictionary<Type, Type[]>();
        public static void UpdateAvailableTagTypes()
        {
            UpdateAvailableTagTypes(typeof(Tag));
        }
        public static Type[] UpdateAvailableTagTypes(Type baseTagType)
        {
            IEnumerable<Type> availableTagTypes = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                ((List<Type>)availableTagTypes).AddRange(Assembly.GetAssembly(baseTagType).GetTypes().Where(t => (t != baseTagType) && baseTagType.IsAssignableFrom(t) && !t.IsAbstract && t.IsPublic));
            availableTagTypes = availableTagTypes.ToArray();
            AvailableTagTypes.Add(baseTagType, ((Type[])availableTagTypes));
            return ((Type[])availableTagTypes);
        }

        public static readonly ObservableCollection<Tag> DetectedTags = new ObservableCollection<Tag>();

        public static Tag NotifyDetection(byte[] uid, Type baseTagType, DetectionSource newDetectionSource)
        {
            //Search for tag types that were already searched,...
            Type[] correpondingTagTypes = null;
            try { correpondingTagTypes = AvailableTagTypes[baseTagType]; }
            //...if not present, do search.
            catch (KeyNotFoundException)
            { correpondingTagTypes = UpdateAvailableTagTypes(baseTagType); }
            //TODO: Not Resilient enough, it do not search for newly introduced types.

            Type tagType = correpondingTagTypes.First(correspondingTagType =>
            {
                try
                {
                    return Activator.CreateInstance(correspondingTagType, uid);
                }
                List<TagModelNumberAttribute> tagModelNumberAttributes = t.GetCustomAttributes(typeof(TagModelNumberAttribute)).Cast<TagModelNumberAttribute>().ToList();
                return tagModelNumberAttributes.Any(tagModelNumberAttribute => tagModelNumberAttribute.UIDCorrespond(uid));
            });

            Tag detectedTag;
            try
            {
                detectedTag = DetectedTags.First(tag => tag.UID.SequenceEqual(uid));

                //Cancel linking with previously detected tag if it is not the same type
                //TODO: Create multi-type tags
                Type previouslyDetectedTagType = detectedTag.GetType();
                if (previouslyDetectedTagType != baseTagType)
                    if (!previouslyDetectedTagType.IsSubclassOf(baseTagType))
                        throw new InvalidOperationException();
            }
            catch (InvalidOperationException)
            {
                if (baseTagType.IsSubclassOf(typeof(Tag)))
                {
                    detectedTag = (Tag)Activator.CreateInstance(tagType, uid);
                    DetectedTags.Add(detectedTag);
                }
                else
                    throw new ArgumentException("Suggested type is not a tag type");
            }

            //Delete old detection source from the same antenna
            try
            {
                DetectionSource previousDetectionSource = detectedTag.DetectionSources.First(detectionSource => detectionSource.Antenna == newDetectionSource.Antenna);
                if (previousDetectionSource.Time < newDetectionSource.Time) //Check time order
                {
                    detectedTag.DetectionSources.Remove(previousDetectionSource);
                    AddAndSortDetectionSource(ref detectedTag, newDetectionSource);
                }
                //Else ignore the current detection as it is anterior of the already present one
            }
            catch (InvalidOperationException)
            {
                AddAndSortDetectionSource(ref detectedTag, newDetectionSource);
            }

            //Update original antenna port with detected tag
            newDetectionSource.Antenna.ConnectedTags.Add(detectedTag);

            return detectedTag;
        }
        /// <summary>
        /// Add detection source to the tag, sorted by RSSI
        /// </summary>
        /// <param name="detectedTag"></param>
        /// <param name="newDetectionSource"></param>
        private static void AddAndSortDetectionSource(ref Tag detectedTag, DetectionSource newDetectionSource)
        {
            int newDetectionSourceSortingPointer = 0;
            DetectionSource previousDetectionSource = null;
            try
            {
                previousDetectionSource = detectedTag.DetectionSources.First(detectionSource => detectionSource.RSSI <= newDetectionSource.RSSI);
            }
            catch (InvalidOperationException) { }
            if (previousDetectionSource != null)
                newDetectionSourceSortingPointer = detectedTag.DetectionSources.IndexOf(previousDetectionSource) - 1;
            if (newDetectionSourceSortingPointer < 0)
                newDetectionSourceSortingPointer = 0;
            detectedTag.DetectionSources.Insert(newDetectionSourceSortingPointer, newDetectionSource);
        }
    }
}
