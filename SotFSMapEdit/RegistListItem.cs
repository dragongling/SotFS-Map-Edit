using System;

namespace SotFSMapEdit
{
	// Token: 0x02000005 RID: 5
	public class RegistListItem
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002192 File Offset: 0x00000392
		// (set) Token: 0x06000018 RID: 24 RVA: 0x0000219A File Offset: 0x0000039A
		public uint REG_ID { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000019 RID: 25 RVA: 0x000021A3 File Offset: 0x000003A3
		// (set) Token: 0x0600001A RID: 26 RVA: 0x000021AB File Offset: 0x000003AB
		public uint SPAWN_ID { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001B RID: 27 RVA: 0x000021B4 File Offset: 0x000003B4
		// (set) Token: 0x0600001C RID: 28 RVA: 0x000021BC File Offset: 0x000003BC
		public byte STATE { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001D RID: 29 RVA: 0x000021C5 File Offset: 0x000003C5
		// (set) Token: 0x0600001E RID: 30 RVA: 0x000021CD File Offset: 0x000003CD
		public byte GROUP { get; set; }
	}
}
