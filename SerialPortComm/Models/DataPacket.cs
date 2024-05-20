namespace RN700Communication.Models
{
	// Parsed Date Example:
	// 24年05月13日
    //14時04分
    //12,台灣精米
    //0,原廠參數
    //00000000
    //1
    //1,1,完整粒,177粒,28.3％,44.4％,0
    //1,0,白粉質粒,18粒,2.9％,4.0％,0
    //1,0,碎粒,346粒,55.4％,45.5％,0
    //1,0,被害粒,0粒,0.0％,0.0％,0
    //1,0,著色粒,1粒,0.2％,0.2％,0
    //1,0,胴裂粒,9粒,1.4％,2.3％,0
    //1,1,小碎粒等,74粒,11.8％,3.7％,0
    //1,0,異種穀粒,0粒,0.0％,0.0％,0
    //1,0,合計,625粒

    public class DataPacket
    {
		private string analysis_date;

		public string AnalysisDate
		{
			get { return analysis_date; }
			set { analysis_date = value; }
		}

		private int myVar;

		public int MyProperty
		{
			get { return myVar; }
			set { myVar = value; }
		}


	}
}
