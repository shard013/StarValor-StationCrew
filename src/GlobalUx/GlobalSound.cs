namespace StationCrew.GlobalUx
{
    public static class GlobalSound
    {
        private static int? QueuedSound { get; set; } = null;

        public static void QueueSound(int soundId)
        {
            QueuedSound = soundId;
        }

        public static void PlaySound(int soundId)
        {
            SoundSys.PlaySound(soundId, true);
        }

        public static void PlayQueuedSound()
        {
            if (QueuedSound.HasValue)
            {
                SoundSys.PlaySound(QueuedSound.Value, true);
                QueuedSound = null;
            }
        }

    }
}
