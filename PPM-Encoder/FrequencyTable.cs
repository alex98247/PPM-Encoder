namespace PPM_Encoder
{
    public interface IFrequencyTable {
        int Get(int symbol);
        void Increment(int symbol);
        int GetTotal();
        int GetLow(int symbol);
        int GetHigh(int symbol);
	
    }
}
