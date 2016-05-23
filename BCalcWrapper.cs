using System;
using System.Runtime.InteropServices;

namespace BCDD
{
    /// <summary>
    /// Wrapper class for libbcalcDDS.ddl.
    /// </summary>
    class BCalcWrapper
    {
        public static char[] DENOMINATIONS = { 'C', 'D', 'H', 'S', 'N' };
        public static char[] PLAYERS = { 'N', 'E', 'S', 'W' };

        /// <remarks>http://bcalc.w8.pl/API_C/bcalcdds_8h.html#ab636045f65412652246b769e8e95ed6f</remarks>
        [DllImport(@"libbcalcdds.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bcalcDDS_new(IntPtr format, IntPtr hands, Int32 trump, Int32 leader);

        /// <remarks>http://bcalc.w8.pl/API_C/bcalcdds_8h.html#a369ce661d027bef3f717967e42bf8b33</remarks>
        [DllImport(@"libbcalcdds.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 bcalcDDS_getTricksToTake(IntPtr solver);

        /// <remarks>http://bcalc.w8.pl/API_C/bcalcdds_8h.html#a89cdec200cde91331d40f0900dc0fb46</remarks>
        [DllImport(@"libbcalcdds.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bcalcDDS_getLastError(IntPtr solver);

        /// <remarks>http://bcalc.w8.pl/API_C/bcalcdds_8h.html#a4a68da83bc7da4663e2257429539912d</remarks>
        [DllImport(@"libbcalcdds.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bcalcDDS_delete(IntPtr solver);

        /// <remarks>http://bcalc.w8.pl/API_C/bcalcdds_8h.html#a88fba3432e66efa5979bbc9e1f044164</remarks>
        [DllImport(@"libbcalcdds.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bcalcDDS_setTrumpAndReset(IntPtr solver, Int32 trump);

        /// <remarks>http://bcalc.w8.pl/API_C/bcalcdds_8h.html#a616031c1e1d856c4aac14390693adb4c</remarks>
        [DllImport(@"libbcalcdds.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bcalcDDS_setPlayerOnLeadAndReset(IntPtr solver, Int32 player);

        /// <remarks>http://bcalc.w8.pl/API_C/bcalcdds_8h.html#a6977a3b789bdf64eb2da9cbdb8b8fc39</remarks>
        public static Int32 bcalc_declarerToLeader(Int32 player)
        {
            return (player + 1) & 3;
        }
    }
}
