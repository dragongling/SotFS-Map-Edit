using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SotFSMapEdit
{
	// Token: 0x02000006 RID: 6
	public partial class MainWindow : Window, IStyleConnector
	{
		// Token: 0x06000020 RID: 32 RVA: 0x000021D8 File Offset: 0x000003D8
		private void RegisterNPC(object sender, RoutedEventArgs e)
		{
			this.RegistWriter.Seek(10, SeekOrigin.Begin);
			this.RegistWriter.Write((byte)((this.REG_FILE_START - 64) / 24 + 1));
			this.REG_FILE_SIZE += 40;
			this.REG_FILE_START += 24;
			this.RegistWriter.Seek(0, SeekOrigin.Begin);
			this.RegistWriter.Write(this.REG_FILE_SIZE);
			this.RegistWriter.Seek(44, SeekOrigin.Current);
			this.RegistWriter.Write(this.REG_FILE_START);
			this.RegistWriter.Seek(20, SeekOrigin.Current);
			int it = 0;
			while (this.RegistWriter.BaseStream.Position < (long)(this.REG_FILE_START - 24))
			{
				this.RegistWriter.Write(this.REG_FILE_START + it * 16);
				this.RegistWriter.Seek(4, SeekOrigin.Current);
				this.RegistWriter.Write(this.REG_FILE_SIZE);
				this.RegistWriter.Seek(12, SeekOrigin.Current);
				it++;
			}
			this.RegistWriter.Close();
			FileStream s = new FileStream("GameDataEbl/param/GeneratorRegistParam_" + this.currentMap, FileMode.Open);
			this.REG_FILE_START -= 24;
			MainWindow.InsertIntoFile(s, (long)this.REG_FILE_START, BitConverter.GetBytes(0));
			MainWindow.InsertIntoFile(s, (long)this.REG_FILE_START, BitConverter.GetBytes(this.REG_FILE_SIZE));
			MainWindow.InsertIntoFile(s, (long)this.REG_FILE_START, BitConverter.GetBytes(0));
			MainWindow.InsertIntoFile(s, (long)this.REG_FILE_START, BitConverter.GetBytes(this.REG_FILE_SIZE - 16));
			MainWindow.InsertIntoFile(s, (long)this.REG_FILE_START, BitConverter.GetBytes(0));
			FileStream stream = s;
			long offset = (long)this.REG_FILE_START;
			int num = this.FINAL_SPAWN_ID + 1;
			this.FINAL_SPAWN_ID = num;
			MainWindow.InsertIntoFile(stream, offset, BitConverter.GetBytes(num));
			int id = int.Parse(this.newEnemyID.Text);
			this.REG_FILE_SIZE -= 16;
			MainWindow.InsertIntoFile(s, (long)this.REG_FILE_SIZE, BitConverter.GetBytes(720896));
			MainWindow.InsertIntoFile(s, (long)this.REG_FILE_SIZE, BitConverter.GetBytes(id / 100 * 100));
			MainWindow.InsertIntoFile(s, (long)this.REG_FILE_SIZE, BitConverter.GetBytes(id / 100 * 100 + 10));
			MainWindow.InsertIntoFile(s, (long)this.REG_FILE_SIZE, BitConverter.GetBytes(id));
			this.REG_FILE_START += 24;
			this.REG_FILE_SIZE += 16;
			s.Close();
			this.RegistWriter = new BinaryWriter(File.Open("GameDataEbl/param/GeneratorRegistParam_" + this.currentMap, FileMode.Open));
			RegistListItem item = new RegistListItem
			{
				REG_ID = uint.Parse(this.newEnemyID.Text),
				SPAWN_ID = (uint)this.FINAL_SPAWN_ID,
				STATE = 0,
				GROUP = 11
			};
			this.RegistList.Items.Add(item);
			this.newEnemyID.Clear();
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000024DC File Offset: 0x000006DC
		private void UnregisterNPC(object sender, RoutedEventArgs e)
		{
			this.RegistList.SelectedItem = this.SELECTED_ITEM_REG;
			bool flag = this.RegistList.SelectedItem == null;
			if (!flag)
			{
				this.RegistWriter.Seek(10, SeekOrigin.Begin);
				this.RegistWriter.Write((byte)((this.REG_FILE_START - 64) / 24 - 1));
				this.REG_FILE_SIZE -= 40;
				this.REG_FILE_START -= 24;
				this.RegistWriter.Seek(0, SeekOrigin.Begin);
				this.RegistWriter.Write(this.REG_FILE_SIZE);
				this.RegistWriter.Seek(44, SeekOrigin.Current);
				this.RegistWriter.Write(this.REG_FILE_START);
				this.RegistWriter.Seek(20, SeekOrigin.Current);
				int it = 0;
				while (this.RegistWriter.BaseStream.Position < (long)this.REG_FILE_START)
				{
					this.RegistWriter.Write(this.REG_FILE_START + it * 16);
					this.RegistWriter.Seek(4, SeekOrigin.Current);
					this.RegistWriter.Write(this.REG_FILE_SIZE);
					this.RegistWriter.Seek(12, SeekOrigin.Current);
					it++;
				}
				this.RegistWriter.Close();
				BinaryReader rr = new BinaryReader(File.Open("GameDataEbl/param/GeneratorRegistParam_" + this.currentMap, FileMode.Open));
				byte[] RegistParam = MainWindow.ReadAllBytes(rr);
				rr.Close();
				List<byte> list = new List<byte>(RegistParam);
				for (int i = 0; i < 24; i++)
				{
					list.RemoveAt(64 + this.RegistList.SelectedIndex * 24);
				}
				for (int j = 0; j < 16; j++)
				{
					list.RemoveAt(this.REG_FILE_START + this.RegistList.SelectedIndex * 16);
				}
				RegistParam = list.ToArray();
				File.Delete("GameDataEbl/param/GeneratorRegistParam_" + this.currentMap);
				this.RegistWriter = new BinaryWriter(File.Open("GameDataEbl/param/GeneratorRegistParam_" + this.currentMap, FileMode.Create));
				this.RegistWriter.Write(RegistParam);
				this.RegistList.Items.Remove(this.RegistList.SelectedItem);
				this.FINAL_SPAWN_ID--;
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002738 File Offset: 0x00000938
		private void ChangeSpawnState(object sender, TextChangedEventArgs e)
		{
			int x;
			bool flag = !int.TryParse(((TextBox)sender).Text, out x);
			if (!flag)
			{
				int index = this.RegistList.SelectedIndex;
				this.RegistWriter.BaseStream.Seek((long)(this.REG_FILE_START + index * 16 + 12), SeekOrigin.Begin);
				this.RegistWriter.Write(byte.Parse(((TextBox)sender).Text));
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000027AC File Offset: 0x000009AC
		private void ChangeDrawGroup(object sender, TextChangedEventArgs e)
		{
			int x;
			bool flag = !int.TryParse(((TextBox)sender).Text, out x);
			if (!flag)
			{
				int index = this.RegistList.SelectedIndex;
				this.RegistWriter.BaseStream.Seek((long)(this.REG_FILE_START + index * 16 + 14), SeekOrigin.Begin);
				this.RegistWriter.Write(byte.Parse(((TextBox)sender).Text));
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002820 File Offset: 0x00000A20
		private void ChangeSpawnID(object sender, TextChangedEventArgs e)
		{
			int x;
			bool flag = !int.TryParse(((TextBox)sender).Text, out x);
			if (!flag)
			{
				int index = this.SpawnList.SelectedIndex;
				this.GeneratorWriter.BaseStream.Seek((long)(this.GEN_FILE_START + index * 160 + 8), SeekOrigin.Begin);
				this.GeneratorWriter.Write(int.Parse(((TextBox)sender).Text));
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002894 File Offset: 0x00000A94
		private void ChangeSpawnLV(object sender, TextChangedEventArgs e)
		{
			int x;
			bool flag = !int.TryParse(((TextBox)sender).Text, out x);
			if (!flag)
			{
				int index = this.SpawnList.SelectedIndex;
				this.GeneratorWriter.BaseStream.Seek((long)(this.GEN_FILE_START + index * 160 + 16), SeekOrigin.Begin);
				this.GeneratorWriter.Write(int.Parse(((TextBox)sender).Text));
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x0000290C File Offset: 0x00000B0C
		private void ChangeSpawnAI(object sender, TextChangedEventArgs e)
		{
			int x;
			bool flag = !int.TryParse(((TextBox)sender).Text, out x);
			if (!flag)
			{
				int index = this.SpawnList.SelectedIndex;
				this.GeneratorWriter.BaseStream.Seek((long)(this.GEN_FILE_START + index * 160 + 88), SeekOrigin.Begin);
				this.GeneratorWriter.Write(int.Parse(((TextBox)sender).Text));
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002984 File Offset: 0x00000B84
		private void ChangeSpawnX(object sender, TextChangedEventArgs e)
		{
			float x;
			bool flag = !float.TryParse(((TextBox)sender).Text, out x);
			if (!flag)
			{
				int index = this.SpawnList.SelectedIndex + ((Tuple<int, int>)((TextBox)sender).Tag).Item2 - ((Tuple<int, int>)((TextBox)sender).Tag).Item1;
				this.LocationWriter.BaseStream.Seek((long)(this.LOC_FILE_START + index * 32 + 8), SeekOrigin.Begin);
				this.LocationWriter.Write(float.Parse(((TextBox)sender).Text) + MainWindow.OriginDictionary[this.currentMap].Item3);
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002A3C File Offset: 0x00000C3C
		private void RegistList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			bool flag = ((ListView)sender).SelectedItems.Count > 0;
			if (flag)
			{
				this.SELECTED_ITEM_REG = ((ListView)sender).SelectedItem;
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002A74 File Offset: 0x00000C74
		private void SpawnList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			bool flag = ((ListView)sender).SelectedItems.Count > 0;
			if (flag)
			{
				this.SELECTED_ITEM_SPAWN = ((ListView)sender).SelectedItem;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002AAC File Offset: 0x00000CAC
		private void ChangeSpawnY(object sender, TextChangedEventArgs e)
		{
			float x;
			bool flag = !float.TryParse(((TextBox)sender).Text, out x);
			if (!flag)
			{
				int index = this.SpawnList.SelectedIndex + ((Tuple<int, int>)((TextBox)sender).Tag).Item2 - ((Tuple<int, int>)((TextBox)sender).Tag).Item1;
				this.LocationWriter.BaseStream.Seek((long)(this.LOC_FILE_START + index * 32), SeekOrigin.Begin);
				this.LocationWriter.Write(float.Parse(((TextBox)sender).Text) + MainWindow.OriginDictionary[this.currentMap].Item1);
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002B60 File Offset: 0x00000D60
		private void ChangeSpawnZ(object sender, TextChangedEventArgs e)
		{
			float x;
			bool flag = !float.TryParse(((TextBox)sender).Text, out x);
			if (!flag)
			{
				int index = this.SpawnList.SelectedIndex + ((Tuple<int, int>)((TextBox)sender).Tag).Item2 - ((Tuple<int, int>)((TextBox)sender).Tag).Item1;
				this.LocationWriter.BaseStream.Seek((long)(this.LOC_FILE_START + index * 32 + 4), SeekOrigin.Begin);
				this.LocationWriter.Write(float.Parse(((TextBox)sender).Text) + MainWindow.OriginDictionary[this.currentMap].Item2);
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002C18 File Offset: 0x00000E18
		private void ChangeSpawnF(object sender, TextChangedEventArgs e)
		{
			float x;
			bool flag = !float.TryParse(((TextBox)sender).Text, out x);
			if (!flag)
			{
				int index = this.SpawnList.SelectedIndex + ((Tuple<int, int>)((TextBox)sender).Tag).Item2 - ((Tuple<int, int>)((TextBox)sender).Tag).Item1;
				this.LocationWriter.BaseStream.Seek((long)(this.LOC_FILE_START + index * 32 + 16), SeekOrigin.Begin);
				this.LocationWriter.Write(float.Parse(((TextBox)sender).Text));
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002CB8 File Offset: 0x00000EB8
		private void SpawnNPC(object sender, RoutedEventArgs e)
		{
			this.SpawnList.SelectedItem = this.SELECTED_ITEM_SPAWN;
			bool flag = this.SpawnList.SelectedItem == null;
			if (!flag)
			{
				this.GeneratorWriter.Seek(10, SeekOrigin.Begin);
				this.GeneratorWriter.Write((byte)((this.GEN_FILE_START - 64) / 24 + 1));
				this.LocationWriter.Seek(10, SeekOrigin.Begin);
				this.LocationWriter.Write((byte)((this.LOC_FILE_START - 64) / 24 + 1));
				this.GEN_FILE_SIZE += 184;
				this.GEN_FILE_START += 24;
				this.LOC_FILE_SIZE += 56;
				this.LOC_FILE_START += 24;
				this.GeneratorWriter.Seek(0, SeekOrigin.Begin);
				this.GeneratorWriter.Write(this.GEN_FILE_SIZE);
				this.GeneratorWriter.Seek(44, SeekOrigin.Current);
				this.GeneratorWriter.Write(this.GEN_FILE_START);
				this.GeneratorWriter.Seek(20, SeekOrigin.Current);
				int it = 0;
				while (this.GeneratorWriter.BaseStream.Position < (long)(this.GEN_FILE_START - 24))
				{
					this.GeneratorWriter.Write(this.GEN_FILE_START + it * 160);
					this.GeneratorWriter.Seek(4, SeekOrigin.Current);
					this.GeneratorWriter.Write(this.GEN_FILE_SIZE);
					this.GeneratorWriter.Seek(12, SeekOrigin.Current);
					it++;
				}
				this.GeneratorWriter.Close();
				this.LocationWriter.Seek(0, SeekOrigin.Begin);
				this.LocationWriter.Write(this.LOC_FILE_SIZE);
				this.LocationWriter.Seek(44, SeekOrigin.Current);
				this.LocationWriter.Write(this.LOC_FILE_START);
				this.LocationWriter.Seek(20, SeekOrigin.Current);
				it = 0;
				while (this.LocationWriter.BaseStream.Position < (long)(this.LOC_FILE_START - 24))
				{
					this.LocationWriter.Write(this.LOC_FILE_START + it * 32);
					this.LocationWriter.Seek(4, SeekOrigin.Current);
					this.LocationWriter.Write(this.LOC_FILE_SIZE);
					this.LocationWriter.Seek(12, SeekOrigin.Current);
					it++;
				}
				this.LocationWriter.Close();
				this.GEN_FILE_START -= 24;
				BinaryReader gr = new BinaryReader(File.Open("GameDataEbl/param/GeneratorParam_" + this.currentMap, FileMode.Open));
				gr.BaseStream.Seek((long)(this.GEN_FILE_START + this.SpawnList.SelectedIndex * 160), SeekOrigin.Begin);
				byte[] GeneratorParam = gr.ReadBytes(160);
				gr.Close();
				FileStream s = new FileStream("GameDataEbl/param/GeneratorParam_" + this.currentMap, FileMode.Open);
				MainWindow.InsertIntoFile(s, (long)this.GEN_FILE_START, BitConverter.GetBytes(0));
				MainWindow.InsertIntoFile(s, (long)this.GEN_FILE_START, BitConverter.GetBytes(this.GEN_FILE_SIZE));
				MainWindow.InsertIntoFile(s, (long)this.GEN_FILE_START, BitConverter.GetBytes(0));
				MainWindow.InsertIntoFile(s, (long)this.GEN_FILE_START, BitConverter.GetBytes(this.GEN_FILE_SIZE - 160));
				MainWindow.InsertIntoFile(s, (long)this.GEN_FILE_START, BitConverter.GetBytes(0));
				FileStream stream = s;
				long offset = (long)this.GEN_FILE_START;
				int num = this.FINAL_ID + 1;
				this.FINAL_ID = num;
				MainWindow.InsertIntoFile(stream, offset, BitConverter.GetBytes(num));
				this.FINAL_INDEX = new Tuple<int, int>(this.FINAL_INDEX.Item1 + 1, this.FINAL_INDEX.Item2 + 1);
				GeneratorParam[0] = 0;
				GeneratorParam[1] = 0;
				GeneratorParam[2] = 0;
				GeneratorParam[3] = (byte)this.FINAL_INDEX.Item1;
				GeneratorParam[44] = 0;
				GeneratorParam[45] = 0;
				GeneratorParam[46] = 0;
				GeneratorParam[47] = 0;
				GeneratorParam[136] = 0;
				GeneratorParam[137] = 0;
				GeneratorParam[138] = 0;
				GeneratorParam[139] = 0;
				MainWindow.InsertIntoFile(s, (long)(this.GEN_FILE_SIZE - 160), GeneratorParam);
				this.GEN_FILE_START += 24;
				s.Close();
				this.LOC_FILE_START -= 24;
				BinaryReader lr = new BinaryReader(File.Open("GameDataEbl/param/GeneratorLocation_" + this.currentMap, FileMode.Open));
				lr.BaseStream.Seek((long)(this.LOC_FILE_START + this.SpawnList.SelectedIndex * 32), SeekOrigin.Begin);
				byte[] LocationParam = lr.ReadBytes(32);
				lr.Close();
				s = new FileStream("GameDataEbl/param/GeneratorLocation_" + this.currentMap, FileMode.Open);
				MainWindow.InsertIntoFile(s, (long)this.LOC_FILE_START, BitConverter.GetBytes(0));
				MainWindow.InsertIntoFile(s, (long)this.LOC_FILE_START, BitConverter.GetBytes(this.LOC_FILE_SIZE));
				MainWindow.InsertIntoFile(s, (long)this.LOC_FILE_START, BitConverter.GetBytes(0));
				MainWindow.InsertIntoFile(s, (long)this.LOC_FILE_START, BitConverter.GetBytes(this.LOC_FILE_SIZE - 32));
				MainWindow.InsertIntoFile(s, (long)this.LOC_FILE_START, BitConverter.GetBytes(0));
				MainWindow.InsertIntoFile(s, (long)this.LOC_FILE_START, BitConverter.GetBytes(this.FINAL_ID));
				MainWindow.InsertIntoFile(s, (long)(this.LOC_FILE_SIZE - 32), LocationParam);
				this.LOC_FILE_START += 24;
				s.Close();
				this.GeneratorWriter = new BinaryWriter(File.Open("GameDataEbl/param/GeneratorParam_" + this.currentMap, FileMode.Open));
				this.LocationWriter = new BinaryWriter(File.Open("GameDataEbl/param/GeneratorLocation_" + this.currentMap, FileMode.Open));
				SpawnListItem item = new SpawnListItem
				{
					INDEX = new Tuple<int, int>(this.FINAL_INDEX.Item1, this.FINAL_INDEX.Item2),
					ID = ((SpawnListItem)this.SpawnList.SelectedItem).ID,
					AI = ((SpawnListItem)this.SpawnList.SelectedItem).AI,
					LV = ((SpawnListItem)this.SpawnList.SelectedItem).LV,
					Y = ((SpawnListItem)this.SpawnList.SelectedItem).Y,
					Z = ((SpawnListItem)this.SpawnList.SelectedItem).Z,
					X = ((SpawnListItem)this.SpawnList.SelectedItem).X,
					F = ((SpawnListItem)this.SpawnList.SelectedItem).F
				};
				this.SpawnList.Items.Add(item);
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x0000334C File Offset: 0x0000154C
		private void DespawnNPC(object sender, RoutedEventArgs e)
		{
			this.SpawnList.SelectedItem = this.SELECTED_ITEM_SPAWN;
			bool flag = this.SpawnList.SelectedItem == null;
			if (!flag)
			{
				this.GeneratorWriter.Seek(10, SeekOrigin.Begin);
				this.GeneratorWriter.Write((byte)((this.GEN_FILE_START - 64) / 24 - 1));
				this.LocationWriter.Seek(10, SeekOrigin.Begin);
				this.LocationWriter.Write((byte)((this.LOC_FILE_START - 64) / 24 - 1));
				this.GEN_FILE_SIZE -= 184;
				this.GEN_FILE_START -= 24;
				this.LOC_FILE_SIZE -= 56;
				this.LOC_FILE_START -= 24;
				this.GeneratorWriter.Seek(0, SeekOrigin.Begin);
				this.GeneratorWriter.Write(this.GEN_FILE_SIZE);
				this.GeneratorWriter.Seek(44, SeekOrigin.Current);
				this.GeneratorWriter.Write(this.GEN_FILE_START);
				this.GeneratorWriter.Seek(20, SeekOrigin.Current);
				int it = 0;
				while (this.GeneratorWriter.BaseStream.Position < (long)this.GEN_FILE_START)
				{
					this.GeneratorWriter.Write(this.GEN_FILE_START + it * 160);
					this.GeneratorWriter.Seek(4, SeekOrigin.Current);
					this.GeneratorWriter.Write(this.GEN_FILE_SIZE);
					this.GeneratorWriter.Seek(12, SeekOrigin.Current);
					it++;
				}
				this.GeneratorWriter.Close();
				this.LocationWriter.Seek(0, SeekOrigin.Begin);
				this.LocationWriter.Write(this.LOC_FILE_SIZE);
				this.LocationWriter.Seek(44, SeekOrigin.Current);
				this.LocationWriter.Write(this.LOC_FILE_START);
				this.LocationWriter.Seek(20, SeekOrigin.Current);
				it = 0;
				while (this.LocationWriter.BaseStream.Position < (long)this.LOC_FILE_START)
				{
					this.LocationWriter.Write(this.LOC_FILE_START + it * 32);
					this.LocationWriter.Seek(4, SeekOrigin.Current);
					this.LocationWriter.Write(this.LOC_FILE_SIZE);
					this.LocationWriter.Seek(12, SeekOrigin.Current);
					it++;
				}
				this.LocationWriter.Close();
				BinaryReader gr = new BinaryReader(File.Open("GameDataEbl/param/GeneratorParam_" + this.currentMap, FileMode.Open));
				byte[] GeneratorParam = MainWindow.ReadAllBytes(gr);
				gr.Close();
				List<byte> list = new List<byte>(GeneratorParam);
				for (int i = 0; i < 24; i++)
				{
					list.RemoveAt(64 + this.SpawnList.SelectedIndex * 24);
				}
				for (int j = 0; j < 160; j++)
				{
					list.RemoveAt(this.GEN_FILE_START + this.SpawnList.SelectedIndex * 160);
				}
				GeneratorParam = list.ToArray();
				File.Delete("GameDataEbl/param/GeneratorParam_" + this.currentMap);
				this.GeneratorWriter = new BinaryWriter(File.Open("GameDataEbl/param/GeneratorParam_" + this.currentMap, FileMode.Create));
				this.GeneratorWriter.Write(GeneratorParam);
				BinaryReader lr = new BinaryReader(File.Open("GameDataEbl/param/GeneratorLocation_" + this.currentMap, FileMode.Open));
				byte[] LocationParam = MainWindow.ReadAllBytes(lr);
				lr.Close();
				list = new List<byte>(LocationParam);
				for (int k = 0; k < 24; k++)
				{
					list.RemoveAt(64 + (this.SpawnList.SelectedIndex + ((SpawnListItem)this.SpawnList.SelectedItem).INDEX.Item2 - ((SpawnListItem)this.SpawnList.SelectedItem).INDEX.Item1) * 24);
				}
				for (int l = 0; l < 32; l++)
				{
					list.RemoveAt(this.LOC_FILE_START + (this.SpawnList.SelectedIndex + ((SpawnListItem)this.SpawnList.SelectedItem).INDEX.Item2 - ((SpawnListItem)this.SpawnList.SelectedItem).INDEX.Item1) * 32);
				}
				LocationParam = list.ToArray();
				File.Delete("GameDataEbl/param/GeneratorLocation_" + this.currentMap);
				this.LocationWriter = new BinaryWriter(File.Open("GameDataEbl/param/GeneratorLocation_" + this.currentMap, FileMode.Create));
				this.LocationWriter.Write(LocationParam);
				this.SpawnList.Items.Remove(this.SpawnList.SelectedItem);
				this.FINAL_INDEX = new Tuple<int, int>(this.FINAL_INDEX.Item1 - 1, this.FINAL_INDEX.Item2 - 1);
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003834 File Offset: 0x00001A34
		public static void InsertIntoFile(FileStream stream, long offset, byte[] extraBytes)
		{
			bool flag = offset < 0L || offset > stream.Length;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("Offset is out of range");
			}
			int bufferSize = 524288;
			long temp = stream.Length - offset;
			bool flag2 = temp <= 524288L;
			if (flag2)
			{
				bufferSize = (int)temp;
			}
			byte[] buffer = new byte[bufferSize];
			long currentPositionToRead = stream.Length;
			bool flag4;
			do
			{
				int numberOfBytesToRead = bufferSize;
				temp = currentPositionToRead - offset;
				bool flag3 = temp < (long)bufferSize;
				if (flag3)
				{
					numberOfBytesToRead = (int)temp;
				}
				currentPositionToRead -= (long)numberOfBytesToRead;
				stream.Position = currentPositionToRead;
				stream.Read(buffer, 0, numberOfBytesToRead);
				stream.Position = currentPositionToRead + (long)extraBytes.Length;
				stream.Write(buffer, 0, numberOfBytesToRead);
				flag4 = (temp <= (long)bufferSize);
			}
			while (!flag4);
			stream.Position = offset;
			stream.Write(extraBytes, 0, extraBytes.Length);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003910 File Offset: 0x00001B10
		public static byte[] ReadAllBytes(BinaryReader reader)
		{
			byte[] result;
			using (MemoryStream ms = new MemoryStream())
			{
				byte[] buffer = new byte[4096];
				int count;
				while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
				{
					ms.Write(buffer, 0, count);
				}
				result = ms.ToArray();
			}
			return result;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003978 File Offset: 0x00001B78
		public MainWindow()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000039AC File Offset: 0x00001BAC
		private void LoadMap(object sender, EventArgs e)
		{
			bool flag = this.currentMap != "";
			if (flag)
			{
				this.GeneratorWriter.Close();
				this.RegistWriter.Close();
				this.LocationWriter.Close();
				this.GeneratorReader.Close();
				this.RegistReader.Close();
				this.LocationReader.Close();
			}
			this.GeneratorWriter = null;
			this.RegistWriter = null;
			this.LocationWriter = null;
			this.GeneratorReader = null;
			this.RegistReader = null;
			this.LocationReader = null;
			this.RegistList.Items.Clear();
			this.SpawnList.Items.Clear();
			this.currentMap = ((ComboBoxItem)this.MapName.SelectedItem).Tag.ToString();
			this.currentMapName = this.MapName.SelectedItem.ToString();
			bool flag2 = this.moddedFiles.IndexOf(this.currentMap) == -1;
			if (flag2)
			{
				this.moddedFiles.Add(this.currentMap);
			}
			BinaryReader gr = new BinaryReader(File.Open("GameDataEbl/param/GeneratorParam_" + this.currentMap, FileMode.Open));
			BinaryReader rr = new BinaryReader(File.Open("GameDataEbl/param/GeneratorRegistParam_" + this.currentMap, FileMode.Open));
			BinaryReader lr = new BinaryReader(File.Open("GameDataEbl/param/GeneratorLocation_" + this.currentMap, FileMode.Open));
			byte[] GeneratorParam = MainWindow.ReadAllBytes(gr);
			byte[] RegistParam = MainWindow.ReadAllBytes(rr);
			byte[] LocationParam = MainWindow.ReadAllBytes(lr);
			gr.Close();
			rr.Close();
			lr.Close();
			this.GeneratorReader = new BinaryReader(new MemoryStream(GeneratorParam));
			this.RegistReader = new BinaryReader(new MemoryStream(RegistParam));
			this.LocationReader = new BinaryReader(new MemoryStream(LocationParam));
			this.LOC_FILE_SIZE = this.LocationReader.ReadInt32();
			int max = this.LOC_FILE_SIZE;
			this.LocationReader.BaseStream.Seek(44L, SeekOrigin.Current);
			this.LOC_FILE_START = this.LocationReader.ReadInt32();
			max -= this.LOC_FILE_START;
			max /= 32;
			this.LocationReader.BaseStream.Seek(12L, SeekOrigin.Current);
			uint[] LOC_ID = new uint[max];
			int i;
			for (i = 0; i < max; i++)
			{
				LOC_ID[i] = this.LocationReader.ReadUInt32();
				this.LocationReader.BaseStream.Seek(20L, SeekOrigin.Current);
			}
			this.FINAL_ID = (int)LOC_ID[max - 1];
			this.GEN_FILE_SIZE = this.GeneratorReader.ReadInt32();
			max = this.GEN_FILE_SIZE;
			this.GeneratorReader.BaseStream.Seek(44L, SeekOrigin.Current);
			this.GEN_FILE_START = this.GeneratorReader.ReadInt32();
			max -= this.GEN_FILE_START;
			max /= 160;
			this.GeneratorReader.BaseStream.Seek(12L, SeekOrigin.Current);
			uint[] GEN_ID = new uint[max];
			for (i = 0; i < max; i++)
			{
				GEN_ID[i] = this.GeneratorReader.ReadUInt32();
				this.GeneratorReader.BaseStream.Seek(20L, SeekOrigin.Current);
			}
			i = 0;
			int j = 0;
			while (i < max)
			{
				this.GeneratorReader.BaseStream.Seek(8L, SeekOrigin.Current);
				uint _ID = this.GeneratorReader.ReadUInt32();
				this.GeneratorReader.BaseStream.Seek(4L, SeekOrigin.Current);
				uint _LV = this.GeneratorReader.ReadUInt32();
				this.GeneratorReader.BaseStream.Seek(68L, SeekOrigin.Current);
				uint _AI = this.GeneratorReader.ReadUInt32();
				while (GEN_ID[i] != LOC_ID[j])
				{
					this.LocationReader.BaseStream.Seek(32L, SeekOrigin.Current);
					j++;
				}
				this.FINAL_INDEX = new Tuple<int, int>(i, j);
				float _Y = this.LocationReader.ReadSingle() - MainWindow.OriginDictionary[this.currentMap].Item1;
				float _Z = this.LocationReader.ReadSingle() - MainWindow.OriginDictionary[this.currentMap].Item2;
				float _X = this.LocationReader.ReadSingle() - MainWindow.OriginDictionary[this.currentMap].Item3;
				this.LocationReader.BaseStream.Seek(4L, SeekOrigin.Current);
				float _F = this.LocationReader.ReadSingle();
				SpawnListItem item = new SpawnListItem
				{
					INDEX = new Tuple<int, int>(i, j),
					ID = _ID,
					AI = _AI,
					LV = _LV,
					Y = _Y,
					Z = _Z,
					X = _X,
					F = _F
				};
				this.SpawnList.Items.Add(item);
				this.GeneratorReader.BaseStream.Seek(68L, SeekOrigin.Current);
				this.LocationReader.BaseStream.Seek(12L, SeekOrigin.Current);
				i++;
				j++;
			}
			this.REG_FILE_SIZE = this.RegistReader.ReadInt32();
			max = this.REG_FILE_SIZE;
			this.RegistReader.BaseStream.Seek(44L, SeekOrigin.Current);
			this.REG_FILE_START = this.RegistReader.ReadInt32();
			max -= this.REG_FILE_START;
			max /= 16;
			this.RegistReader.BaseStream.Seek(12L, SeekOrigin.Current);
			List<int> SpawnIDList = new List<int>();
			while (this.RegistReader.BaseStream.Position < (long)this.REG_FILE_START)
			{
				SpawnIDList.Add(this.RegistReader.ReadInt32());
				this.RegistReader.BaseStream.Seek(20L, SeekOrigin.Current);
			}
			this.FINAL_SPAWN_ID = SpawnIDList[SpawnIDList.Count - 1];
			for (int it = 0; it < max; it++)
			{
				uint reg_id = this.RegistReader.ReadUInt32();
				this.RegistReader.BaseStream.Seek(8L, SeekOrigin.Current);
				byte state = this.RegistReader.ReadByte();
				this.RegistReader.BaseStream.Seek(1L, SeekOrigin.Current);
				byte group = this.RegistReader.ReadByte();
				RegistListItem item2 = new RegistListItem
				{
					REG_ID = reg_id,
					SPAWN_ID = (uint)SpawnIDList[it],
					STATE = state,
					GROUP = group
				};
				this.RegistList.Items.Add(item2);
				this.RegistReader.BaseStream.Seek(1L, SeekOrigin.Current);
			}
			this.GeneratorWriter = new BinaryWriter(File.Open("GameDataEbl/param/GeneratorParam_" + this.currentMap, FileMode.Open));
			this.RegistWriter = new BinaryWriter(File.Open("GameDataEbl/param/GeneratorRegistParam_" + this.currentMap, FileMode.Open));
			this.LocationWriter = new BinaryWriter(File.Open("GameDataEbl/param/GeneratorLocation_" + this.currentMap, FileMode.Open));
			this.L1.IsEnabled = true;
			this.L2.IsEnabled = true;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000040E8 File Offset: 0x000022E8
		private void Repack(object sender, EventArgs e)
		{
			bool flag = this.moddedFiles.Count < 1;
			if (!flag)
			{
				this.GeneratorWriter.Close();
				this.RegistWriter.Close();
				this.LocationWriter.Close();
				for (int i = 0; i < this.moddedFiles.Count; i++)
				{
					Process GenRepack = new Process();
					GenRepack.StartInfo.FileName = "SotFSRepack.exe";
					GenRepack.StartInfo.Arguments = string.Concat(new object[]
					{
						"GameDataEbl/param/GeneratorParam_",
						this.moddedFiles[i],
						" ",
						MainWindow.RepackDictionary["GeneratorParam_" + this.moddedFiles[i]].Item1,
						" ",
						MainWindow.RepackDictionary["GeneratorParam_" + this.moddedFiles[i]].Item2
					});
					GenRepack.Start();
					GenRepack.WaitForExit();
					Process RegistRepack = new Process();
					RegistRepack.StartInfo.FileName = "SotFSRepack.exe";
					RegistRepack.StartInfo.Arguments = string.Concat(new object[]
					{
						"GameDataEbl/param/GeneratorRegistParam_",
						this.moddedFiles[i],
						" ",
						MainWindow.RepackDictionary["GeneratorRegistParam_" + this.moddedFiles[i]].Item1,
						" ",
						MainWindow.RepackDictionary["GeneratorRegistParam_" + this.moddedFiles[i]].Item2
					});
					RegistRepack.Start();
					RegistRepack.WaitForExit();
					Process LocRepack = new Process();
					LocRepack.StartInfo.FileName = "SotFSRepack.exe";
					LocRepack.StartInfo.Arguments = string.Concat(new object[]
					{
						"GameDataEbl/param/GeneratorLocation_",
						this.moddedFiles[i],
						" ",
						MainWindow.RepackDictionary["GeneratorLocation_" + this.moddedFiles[i]].Item1,
						" ",
						MainWindow.RepackDictionary["GeneratorLocation_" + this.moddedFiles[i]].Item2
					});
					LocRepack.Start();
					LocRepack.WaitForExit();
				}
				this.GeneratorWriter = new BinaryWriter(File.Open("GameDataEbl/param/GeneratorParam_" + this.currentMap, FileMode.Open));
				this.RegistWriter = new BinaryWriter(File.Open("GameDataEbl/param/GeneratorRegistParam_" + this.currentMap, FileMode.Open));
				this.LocationWriter = new BinaryWriter(File.Open("GameDataEbl/param/GeneratorLocation_" + this.currentMap, FileMode.Open));
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000045DC File Offset: 0x000027DC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 6:
				((TextBox)target).TextChanged += this.ChangeSpawnState;
				break;
			case 7:
				((TextBox)target).TextChanged += this.ChangeDrawGroup;
				break;
			case 13:
				((TextBox)target).TextChanged += this.ChangeSpawnID;
				break;
			case 14:
				((TextBox)target).TextChanged += this.ChangeSpawnLV;
				break;
			case 15:
				((TextBox)target).TextChanged += this.ChangeSpawnX;
				break;
			case 16:
				((TextBox)target).TextChanged += this.ChangeSpawnY;
				break;
			case 17:
				((TextBox)target).TextChanged += this.ChangeSpawnZ;
				break;
			case 18:
				((TextBox)target).TextChanged += this.ChangeSpawnF;
				break;
			case 19:
				((TextBox)target).TextChanged += this.ChangeSpawnAI;
				break;
			}
		}

		// Token: 0x0400000F RID: 15
		private BinaryReader GeneratorReader;

		// Token: 0x04000010 RID: 16
		private BinaryReader RegistReader;

		// Token: 0x04000011 RID: 17
		private BinaryReader LocationReader;

		// Token: 0x04000012 RID: 18
		private BinaryWriter GeneratorWriter;

		// Token: 0x04000013 RID: 19
		private BinaryWriter RegistWriter;

		// Token: 0x04000014 RID: 20
		private BinaryWriter LocationWriter;

		// Token: 0x04000015 RID: 21
		private string currentMap = "";

		// Token: 0x04000016 RID: 22
		private string currentMapName = "";

		// Token: 0x04000017 RID: 23
		private List<string> moddedFiles = new List<string>();

		// Token: 0x04000018 RID: 24
		private int GEN_FILE_SIZE;

		// Token: 0x04000019 RID: 25
		private int REG_FILE_SIZE;

		// Token: 0x0400001A RID: 26
		private int LOC_FILE_SIZE;

		// Token: 0x0400001B RID: 27
		private int GEN_FILE_START;

		// Token: 0x0400001C RID: 28
		private int REG_FILE_START;

		// Token: 0x0400001D RID: 29
		private int LOC_FILE_START;

		// Token: 0x0400001E RID: 30
		private int FINAL_ID;

		// Token: 0x0400001F RID: 31
		private int FINAL_SPAWN_ID;

		// Token: 0x04000020 RID: 32
		private object SELECTED_ITEM_REG;

		// Token: 0x04000021 RID: 33
		private object SELECTED_ITEM_SPAWN;

		// Token: 0x04000022 RID: 34
		private Tuple<int, int> FINAL_INDEX;

		// Token: 0x04000023 RID: 35
		public static Dictionary<string, Tuple<float, float, float>> OriginDictionary = new Dictionary<string, Tuple<float, float, float>>
		{
			{
				"m10_02_00_00.param",
				new Tuple<float, float, float>(498f, -30.375f, 260f)
			},
			{
				"m10_04_00_00.param",
				new Tuple<float, float, float>(0f, 0f, 0f)
			},
			{
				"m10_10_00_00.param",
				new Tuple<float, float, float>(-208f, -10f, 133f)
			},
			{
				"m10_14_00_00.param",
				new Tuple<float, float, float>(631f, -94f, 154f)
			},
			{
				"m10_15_00_00.param",
				new Tuple<float, float, float>(598f, -57f, 154f)
			},
			{
				"m10_16_00_00.param",
				new Tuple<float, float, float>(78f, -4f, -562f)
			},
			{
				"m10_17_00_00.param",
				new Tuple<float, float, float>(644f, -17.9f, -314f)
			},
			{
				"m10_18_00_00.param",
				new Tuple<float, float, float>(-52f, 70f, -487f)
			},
			{
				"m10_19_00_00.param",
				new Tuple<float, float, float>(589f, -166f, -605f)
			},
			{
				"m10_23_00_00.param",
				new Tuple<float, float, float>(166f, 11f, 10.235f)
			},
			{
				"m10_25_00_00.param",
				new Tuple<float, float, float>(-188f, 150f, 40f)
			},
			{
				"m10_27_00_00.param",
				new Tuple<float, float, float>(712f, -11.5f, 434f)
			},
			{
				"m10_29_00_00.param",
				new Tuple<float, float, float>(124f, -19f, 103f)
			},
			{
				"m10_30_00_00.param",
				new Tuple<float, float, float>(-8.7f, 11f, -358f)
			},
			{
				"m10_31_00_00.param",
				new Tuple<float, float, float>(14f, 15f, -163f)
			},
			{
				"m10_32_00_00.param",
				new Tuple<float, float, float>(488f, -55f, 587f)
			},
			{
				"m10_33_00_00.param",
				new Tuple<float, float, float>(479f, 9f, -211f)
			},
			{
				"m10_34_00_00.param",
				new Tuple<float, float, float>(323f, 74.5f, -38.65f)
			},
			{
				"m20_10_00_00.param",
				new Tuple<float, float, float>(-206f, 4f, 175.6f)
			},
			{
				"m20_11_00_00.param",
				new Tuple<float, float, float>(903f, 29f, 242f)
			},
			{
				"m20_21_00_00.param",
				new Tuple<float, float, float>(356f, 28f, 363f)
			},
			{
				"m20_24_00_00.param",
				new Tuple<float, float, float>(1154.3f, 156.5f, 158.2f)
			},
			{
				"m20_26_00_00.param",
				new Tuple<float, float, float>(0f, 0f, 0f)
			},
			{
				"m40_03_00_00.param",
				new Tuple<float, float, float>(0f, 0f, 0f)
			},
			{
				"m50_35_00_00.param",
				new Tuple<float, float, float>(0f, 0f, 0f)
			},
			{
				"m50_36_00_00.param",
				new Tuple<float, float, float>(0f, 0f, 0f)
			},
			{
				"m50_37_00_00.param",
				new Tuple<float, float, float>(0f, 0f, 0f)
			},
			{
				"m50_38_00_00.param",
				new Tuple<float, float, float>(1099.9f, 156.65f, 172.6f)
			}
		};

		// Token: 0x04000024 RID: 36
		public static Dictionary<string, Tuple<int, int>> RepackDictionary = new Dictionary<string, Tuple<int, int>>
		{
			{
				"GeneratorParam_m10_04_00_00.param",
				new Tuple<int, int>(286, 5)
			},
			{
				"GeneratorParam_m10_16_00_00.param",
				new Tuple<int, int>(330, 5)
			},
			{
				"GeneratorParam_m10_17_00_00.param",
				new Tuple<int, int>(344, 3)
			},
			{
				"GeneratorParam_m10_18_00_00.param",
				new Tuple<int, int>(358, 6)
			},
			{
				"GeneratorParam_m10_27_00_00.param",
				new Tuple<int, int>(360, 5)
			},
			{
				"GeneratorParam_m10_29_00_00.param",
				new Tuple<int, int>(388, 4)
			},
			{
				"GeneratorParam_m10_19_00_00.param",
				new Tuple<int, int>(779, 5)
			},
			{
				"GeneratorParam_m10_30_00_00.param",
				new Tuple<int, int>(988, 3)
			},
			{
				"GeneratorParam_m10_31_00_00.param",
				new Tuple<int, int>(1002, 4)
			},
			{
				"GeneratorParam_m10_32_00_00.param",
				new Tuple<int, int>(1016, 3)
			},
			{
				"GeneratorParam_m50_35_00_00.param",
				new Tuple<int, int>(1225, 5)
			},
			{
				"GeneratorParam_m20_10_00_00.param",
				new Tuple<int, int>(1303, 5)
			},
			{
				"GeneratorParam_m20_11_00_00.param",
				new Tuple<int, int>(1317, 6)
			},
			{
				"GeneratorParam_m20_21_00_00.param",
				new Tuple<int, int>(1333, 3)
			},
			{
				"GeneratorParam_m10_10_00_00.param",
				new Tuple<int, int>(1363, 6)
			},
			{
				"GeneratorParam_m10_23_00_00.param",
				new Tuple<int, int>(1421, 10)
			},
			{
				"GeneratorParam_m10_33_00_00.param",
				new Tuple<int, int>(1437, 5)
			},
			{
				"GeneratorParam_m10_34_00_00.param",
				new Tuple<int, int>(1451, 2)
			},
			{
				"GeneratorParam_m40_03_00_00.param",
				new Tuple<int, int>(1616, 3)
			},
			{
				"GeneratorParam_m50_36_00_00.param",
				new Tuple<int, int>(1646, 3)
			},
			{
				"GeneratorParam_m50_37_00_00.param",
				new Tuple<int, int>(1660, 10)
			},
			{
				"GeneratorParam_m50_38_00_00.param",
				new Tuple<int, int>(1674, 1)
			},
			{
				"GeneratorParam_m10_02_00_00.param",
				new Tuple<int, int>(1782, 7)
			},
			{
				"GeneratorParam_m20_24_00_00.param",
				new Tuple<int, int>(1782, 11)
			},
			{
				"GeneratorParam_m20_26_00_00.param",
				new Tuple<int, int>(1810, 4)
			},
			{
				"GeneratorParam_m10_14_00_00.param",
				new Tuple<int, int>(1826, 5)
			},
			{
				"GeneratorParam_m10_15_00_00.param",
				new Tuple<int, int>(1840, 6)
			},
			{
				"GeneratorParam_m10_25_00_00.param",
				new Tuple<int, int>(1856, 4)
			},
			{
				"GeneratorRegistParam_m10_04_00_00.param",
				new Tuple<int, int>(146, 2)
			},
			{
				"GeneratorRegistParam_m10_16_00_00.param",
				new Tuple<int, int>(190, 2)
			},
			{
				"GeneratorRegistParam_m10_17_00_00.param",
				new Tuple<int, int>(204, 6)
			},
			{
				"GeneratorRegistParam_m10_18_00_00.param",
				new Tuple<int, int>(218, 3)
			},
			{
				"GeneratorRegistParam_m10_27_00_00.param",
				new Tuple<int, int>(220, 8)
			},
			{
				"GeneratorRegistParam_m10_29_00_00.param",
				new Tuple<int, int>(248, 5)
			},
			{
				"GeneratorRegistParam_m10_19_00_00.param",
				new Tuple<int, int>(639, 4)
			},
			{
				"GeneratorRegistParam_m10_30_00_00.param",
				new Tuple<int, int>(848, 7)
			},
			{
				"GeneratorRegistParam_m10_31_00_00.param",
				new Tuple<int, int>(862, 7)
			},
			{
				"GeneratorRegistParam_m10_32_00_00.param",
				new Tuple<int, int>(876, 8)
			},
			{
				"GeneratorRegistParam_m50_35_00_00.param",
				new Tuple<int, int>(1085, 8)
			},
			{
				"GeneratorRegistParam_m20_10_00_00.param",
				new Tuple<int, int>(1163, 3)
			},
			{
				"GeneratorRegistParam_m20_11_00_00.param",
				new Tuple<int, int>(1177, 9)
			},
			{
				"GeneratorRegistParam_m20_21_00_00.param",
				new Tuple<int, int>(1193, 7)
			},
			{
				"GeneratorRegistParam_m10_10_00_00.param",
				new Tuple<int, int>(1223, 6)
			},
			{
				"GeneratorRegistParam_m10_23_00_00.param",
				new Tuple<int, int>(1281, 2)
			},
			{
				"GeneratorRegistParam_m10_33_00_00.param",
				new Tuple<int, int>(1297, 4)
			},
			{
				"GeneratorRegistParam_m10_34_00_00.param",
				new Tuple<int, int>(1311, 1)
			},
			{
				"GeneratorRegistParam_m40_03_00_00.param",
				new Tuple<int, int>(1476, 3)
			},
			{
				"GeneratorRegistParam_m50_36_00_00.param",
				new Tuple<int, int>(1506, 5)
			},
			{
				"GeneratorRegistParam_m50_37_00_00.param",
				new Tuple<int, int>(1520, 7)
			},
			{
				"GeneratorRegistParam_m50_38_00_00.param",
				new Tuple<int, int>(1534, 6)
			},
			{
				"GeneratorRegistParam_m10_02_00_00.param",
				new Tuple<int, int>(1642, 3)
			},
			{
				"GeneratorRegistParam_m20_24_00_00.param",
				new Tuple<int, int>(1642, 5)
			},
			{
				"GeneratorRegistParam_m20_26_00_00.param",
				new Tuple<int, int>(1670, 5)
			},
			{
				"GeneratorRegistParam_m10_14_00_00.param",
				new Tuple<int, int>(1686, 3)
			},
			{
				"GeneratorRegistParam_m10_15_00_00.param",
				new Tuple<int, int>(1700, 4)
			},
			{
				"GeneratorRegistParam_m10_25_00_00.param",
				new Tuple<int, int>(1716, 6)
			},
			{
				"GeneratorLocation_m20_10_00_00.param",
				new Tuple<int, int>(65, 5)
			},
			{
				"GeneratorLocation_m20_11_00_00.param",
				new Tuple<int, int>(79, 4)
			},
			{
				"GeneratorLocation_m20_21_00_00.param",
				new Tuple<int, int>(95, 3)
			},
			{
				"GeneratorLocation_m10_10_00_00.param",
				new Tuple<int, int>(125, 2)
			},
			{
				"GeneratorLocation_m10_32_00_00.param",
				new Tuple<int, int>(185, 5)
			},
			{
				"GeneratorLocation_m10_33_00_00.param",
				new Tuple<int, int>(199, 6)
			},
			{
				"GeneratorLocation_m10_34_00_00.param",
				new Tuple<int, int>(213, 5)
			},
			{
				"GeneratorLocation_m50_35_00_00.param",
				new Tuple<int, int>(394, 6)
			},
			{
				"GeneratorLocation_m50_36_00_00.param",
				new Tuple<int, int>(408, 9)
			},
			{
				"GeneratorLocation_m50_37_00_00.param",
				new Tuple<int, int>(422, 6)
			},
			{
				"GeneratorLocation_m10_02_00_00.param",
				new Tuple<int, int>(544, 6)
			},
			{
				"GeneratorLocation_m20_24_00_00.param",
				new Tuple<int, int>(544, 8)
			},
			{
				"GeneratorLocation_m10_14_00_00.param",
				new Tuple<int, int>(588, 5)
			},
			{
				"GeneratorLocation_m10_23_00_00.param",
				new Tuple<int, int>(590, 4)
			},
			{
				"GeneratorLocation_m10_25_00_00.param",
				new Tuple<int, int>(618, 5)
			},
			{
				"GeneratorLocation_m40_03_00_00.param",
				new Tuple<int, int>(785, 5)
			},
			{
				"GeneratorLocation_m50_38_00_00.param",
				new Tuple<int, int>(843, 6)
			},
			{
				"GeneratorLocation_m10_04_00_00.param",
				new Tuple<int, int>(979, 6)
			},
			{
				"GeneratorLocation_m20_26_00_00.param",
				new Tuple<int, int>(979, 8)
			},
			{
				"GeneratorLocation_m10_15_00_00.param",
				new Tuple<int, int>(1009, 5)
			},
			{
				"GeneratorLocation_m10_16_00_00.param",
				new Tuple<int, int>(1023, 3)
			},
			{
				"GeneratorLocation_m10_17_00_00.param",
				new Tuple<int, int>(1037, 6)
			},
			{
				"GeneratorLocation_m10_27_00_00.param",
				new Tuple<int, int>(1053, 5)
			},
			{
				"GeneratorLocation_m10_18_00_00.param",
				new Tuple<int, int>(1458, 1)
			},
			{
				"GeneratorLocation_m10_19_00_00.param",
				new Tuple<int, int>(1472, 5)
			},
			{
				"GeneratorLocation_m10_29_00_00.param",
				new Tuple<int, int>(1488, 4)
			},
			{
				"GeneratorLocation_m10_30_00_00.param",
				new Tuple<int, int>(1681, 7)
			},
			{
				"GeneratorLocation_m10_31_00_00.param",
				new Tuple<int, int>(1695, 5)
			}
		};
	}
}
