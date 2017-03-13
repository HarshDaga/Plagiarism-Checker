using System;
using FlowGraph;
using GalaSoft.MvvmLight;

namespace GUI.Model
{
	public class ResultItem : ViewModelBase
	{
		private GFunction gFunc1;
		private GFunction gFunc2;
		private string file1;
		private string file2;
		private decimal percentage;

		public string File1
		{
			get => this.file1;
			set => Set ( nameof ( File1 ), ref this.file1, value );
		}

		public string File2
		{
			get => this.file2;
			set => Set ( nameof ( File2 ), ref this.file2, value );
		}

		public decimal Percentage
		{
			get => this.percentage;
			set => Set ( nameof ( Percentage ), ref this.percentage, value );
		}

		public string StrPercentage => $"{percentage}%";

		public ResultItem ( GFunction lhs, GFunction rhs )
		{
			gFunc1 = lhs;
			gFunc2 = rhs;
			File1 = lhs.FileName;
			File2 = rhs.FileName;
			Percentage = Math.Round ( lhs.Compare ( rhs ), 2 );
		}
	}
}