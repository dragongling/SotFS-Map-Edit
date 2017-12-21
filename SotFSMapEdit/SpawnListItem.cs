using System;

namespace SotFSMapEdit
{
	// Token: 0x02000004 RID: 4
	public class SpawnListItem
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002101 File Offset: 0x00000301
		// (set) Token: 0x06000007 RID: 7 RVA: 0x00002109 File Offset: 0x00000309
		public Tuple<int, int> INDEX { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002112 File Offset: 0x00000312
		// (set) Token: 0x06000009 RID: 9 RVA: 0x0000211A File Offset: 0x0000031A
		public uint ID { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002123 File Offset: 0x00000323
		// (set) Token: 0x0600000B RID: 11 RVA: 0x0000212B File Offset: 0x0000032B
		public uint LV { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002134 File Offset: 0x00000334
		// (set) Token: 0x0600000D RID: 13 RVA: 0x0000213C File Offset: 0x0000033C
		public uint AI { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002145 File Offset: 0x00000345
		// (set) Token: 0x0600000F RID: 15 RVA: 0x0000214D File Offset: 0x0000034D
		public float X { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00002156 File Offset: 0x00000356
		// (set) Token: 0x06000011 RID: 17 RVA: 0x0000215E File Offset: 0x0000035E
		public float Y { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002167 File Offset: 0x00000367
		// (set) Token: 0x06000013 RID: 19 RVA: 0x0000216F File Offset: 0x0000036F
		public float Z { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000014 RID: 20 RVA: 0x00002178 File Offset: 0x00000378
		// (set) Token: 0x06000015 RID: 21 RVA: 0x00002180 File Offset: 0x00000380
		public float F { get; set; }
	}
}
