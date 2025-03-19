using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpRESTAtribute : System.Attribute
{
	public string URL { get; set; }
	public HttpRESTAtribute(string URL)

	{

		this.URL = URL;

	}
}
