using TrueGearSDK;


namespace MyTrueGear
{
    public class TrueGearMod
    {
        private static TrueGearPlayer _player = null;

        public TrueGearMod() 
        {
            _player = new TrueGearPlayer("2777220","Human Fall Flat VR");
            _player.Start();
        }
        public void Play(string Event)
        { 
            _player.SendPlay(Event);
        }

    }
}
