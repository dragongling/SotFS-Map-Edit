using System;
using System.Windows;
using System.Windows.Controls;

namespace SotFSMapEdit
{
	// Token: 0x02000003 RID: 3
	public class EditBox : Control
	{
		// Token: 0x04000001 RID: 1
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(EditBox), new FrameworkPropertyMetadata());

		// Token: 0x04000002 RID: 2
		public static DependencyProperty IsEditingProperty = DependencyProperty.Register("IsEditing", typeof(bool), typeof(EditBox), new FrameworkPropertyMetadata(false));
	}
}
