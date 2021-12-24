using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RayForm : MonoBehaviour
{
	//stats
	private Spells.Spell spell;
	private CustomMesh mesh1, mesh2;
	public float power;

	//references
	private Transform myTransform;
	private Renderer myRenderer;

    private void Start()
    {
		power = MagicBalance.Ray.power;
	}

    public void SetStats(Spells.Spell spell)
	{
		this.spell = spell;
		myTransform = GetComponent<Transform>();
		myRenderer = GetComponent<Renderer>();

		Debug.Log("t/rayForm: RayForm.SetStats()");

		switch (spell.currentType)
		{
			case Spells.MagicTypes.Fire:
				Debug.Log("t/rayForm: RaySpellType: fire");
				transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1f, 0, 0, 0.65f);
				transform.GetChild(1).GetComponent<Renderer>().material.color = new Color(1f, 0, 0, 0.65f);
				break;
			case Spells.MagicTypes.Wind:
				transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.33f);
				transform.GetChild(1).GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.33f);
				break;
			case Spells.MagicTypes.Water:
				transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(0, 0, 1f, 0.33f);
				transform.GetChild(1).GetComponent<Renderer>().material.color = new Color(0, 0, 1f, 0.33f);
				break;
			case Spells.MagicTypes.Earth:
				transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(0.806f, 0.533f, 0.125f, 1f);
				transform.GetChild(1).GetComponent<Renderer>().material.color = new Color(0.806f, 0.533f, 0.125f, 1f);
				break;
			case Spells.MagicTypes.Default:
				break;
		}

		mesh1 = new CustomMesh(transform.GetChild(0).GetComponent<MeshFilter>(), myTransform, myRenderer, spell);// do they need renderers?
		mesh2 = new CustomMesh(transform.GetChild(1).GetComponent<MeshFilter>(), myTransform, myRenderer, spell);
	}

    private void FixedUpdate()
    {
		mesh1.UpdateCustomMesh();
		mesh2.UpdateCustomMesh();
	}

    private class CustomMesh
    {
		private Spells.Spell spell;

		private Mesh mesh;
		private int[] triangles;
		private Vector3[] vertices;

		public int distance = 0;
		public int maxDistance = 10;
		public float speed;

		public int tubeAmount = 15;

		private int activeLvls = 1;
		private float currentDistance = 0;

		

		//references
		private MeshFilter meshFilter;
		private Transform myTransform;
		private Renderer myRenderer;

        public CustomMesh(MeshFilter meshFilter, Transform myTransform, Renderer myRenderer, Spells.Spell spell)
        {
			maxDistance = MagicBalance.Ray.maxDistance;
			speed = MagicBalance.Ray.speed;

			this.meshFilter = meshFilter;
			this.myTransform = myTransform;
			this.myRenderer = myRenderer;
			this.spell = spell;

			Debug.Log("t/rayForm: Start.");

			

			mesh = new Mesh();
			meshFilter.mesh = mesh;

			CreateShape();

			//change shape a little
			myTransform.localScale = new Vector3(0.3f, 0.3f, 1);

			Debug.Log("t/rayForm: RaySpellType: choosing...");

		}

		public void UpdateCustomMesh()
        {
			if (activeLvls >= tubeAmount)
			{
				MoveVertices(tubeAmount, speed);
			}
			else
			{
				MoveVerticesStart(tubeAmount, speed);
			}
			UpdateMesh();
		}

		private void CreateShape()
		{
			vertices = CalculateRandomVertices(tubeAmount);
			triangles = CalculateTriangles(tubeAmount);
		}

		private Vector3[] CalculateRandomVertices(int tubeCount)
		{


			Vector3[] vertices = new Vector3[(tubeCount + 1) * 4];

			Debug.Log("t/rayForm: Verticles calculated. VerticlesCount: " + vertices.Length);

			vertices[0] = new Vector3(
				Random.Range(-1f, 0f),
				Random.Range(-1f, 0f),
				0);
			vertices[0 + 1] = new Vector3(
				Random.Range(0f, 1f),
				Random.Range(-1f, 0f),
				0);
			vertices[0 + 2] = new Vector3(
				Random.Range(-1f, 0f),
				Random.Range(0f, 1f),
				0);
			vertices[0 + 3] = new Vector3(
				Random.Range(0f, 1f),
				Random.Range(0f, 1f),
				0);

			int j = 0;
			for (int i = 4; i < vertices.Length - 4; i += 8)
			{
				vertices[i] = new Vector3(
					Random.Range(-1f, 0f),
					Random.Range(-1f, 0f),
					j);
				vertices[i + 1] = new Vector3(Random.Range(0f, 1f),
					Random.Range(-1f, 0f),
					j);
				vertices[i + 2] = new Vector3(
					Random.Range(-1f, 0f),
					Random.Range(0f, 1f),
					j);
				vertices[i + 3] = new Vector3(
					Random.Range(0f, 1f),
					Random.Range(0f, 1f),
					j);

				vertices[i + 4] = vertices[i];
				vertices[i + 5] = vertices[i + 1];
				vertices[i + 6] = vertices[i + 2];
				vertices[i + 7] = vertices[i + 3];

				//j++;
			}

			vertices[vertices.Length - 4] = new Vector3(
				Random.Range(-1f, 0f),
				Random.Range(-1f, 0f),
				j);
			vertices[vertices.Length - 3] = new Vector3(
				Random.Range(0f, 1f),
				Random.Range(-1f, 0f),
				j);
			vertices[vertices.Length - 2] = new Vector3(
				Random.Range(-1f, 0f),
				Random.Range(0f, 1f),
				j);
			vertices[vertices.Length - 1] = new Vector3(
				Random.Range(0f, 1f),
				Random.Range(0f, 1f),
				j);
			return vertices;
		}

		private Vector3[] CalculateRandom4Vertices()
		{
			Debug.Log("t/rayForm: Random 4 verticles calculated.");

			Vector3[] nv = new Vector3[4];

			nv[0] = new Vector3(
				Random.Range(-1f, 0f),
				Random.Range(-1f, 0f),
				0);
			nv[0 + 1] = new Vector3(
				Random.Range(0f, 1f),
				Random.Range(-1f, 0f),
				0);
			nv[0 + 2] = new Vector3(
				Random.Range(-1f, 0f),
				Random.Range(0f, 1f),
				0);
			nv[0 + 3] = new Vector3(
				Random.Range(0f, 1f),
				Random.Range(0f, 1f),
				0);

			return nv;
		}

		private int[] CalculateTriangles(int tubeCount)
		{
			int[] triangles = new int[24 * tubeCount];

			Debug.Log("t/rayForm: Triangles calculated. TrianglesCount: " + triangles.Length);

			for (int i = 0; i < tubeCount; i++)
			{
				int[] tube = CalculateTube(i * 4);
				for (int j = 0; j < 24; j++)
				{
					triangles[i * 24 + j] = tube[j];
				}
			}

			return triangles;
		}

		/// <summary>
		/// give 8 points, take array of connected triangles as tube (24 intigers)
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		private int[] CalculateTube(int startNumber)
		{
			Debug.Log("t/rayForm: Calculating tube...");

			int[] newTriangles = new int[3 * 8]
			{
			3+startNumber,6+startNumber,2+startNumber,
			3+startNumber,7+startNumber,6+startNumber,

			1+startNumber,7+startNumber,3+startNumber,
			1+startNumber,5+startNumber,7+startNumber,

			0+startNumber,5+startNumber,1+startNumber,
			0+startNumber,4+startNumber,5+startNumber,

			2+startNumber,4+startNumber,0+startNumber,
			2+startNumber,6+startNumber,4+startNumber
			};

			return newTriangles;
		}
		private void MoveVerticesStart(int tubeCount, float speed)
		{
			//Debug.Log("t/rayForm: RayForm start to grow...");

			for (int i = 0; i < activeLvls * 4; i += 4)
			{
				if (i == 0) currentDistance += speed;

				vertices[i].z += speed;
				vertices[i + 1].z += speed;
				vertices[i + 2].z += speed;
				vertices[i + 3].z += speed;
			}

			if (currentDistance >= maxDistance / 10)
			{
				Debug.Log("t/rayForm: RayForm growed by 1 unit.");
				currentDistance = 0;
				distance++;
				activeLvls++;
			}
		}

		private void MoveVertices(int tubeCount, float speed)
		{
			//Debug.Log("t/rayForm: MoveVertices(); ");

			for (int i = 0; i < activeLvls * 4; i += 4)
			{
				if (i == 0) currentDistance += speed;
				if (i == activeLvls * 4) break;

				vertices[i].z += speed;
				vertices[i + 1].z += speed;
				vertices[i + 2].z += speed;
				vertices[i + 3].z += speed;
			}

			if (currentDistance >= maxDistance / 10)
			{
				Debug.Log("t/rayForm: RayForm moved by 1 unit.");
				currentDistance = 0;
				DeleteVerticesNew();
			}
		}

		private void DeleteVerticesNew()
		{
			//Create new 4 verteces
			Vector3[] newVerteces = CalculateRandom4Vertices();

			//Make free place for new verteces and triangles on beginning of arrays
			// vertecis.length = 72
			// triangles.length = 216

			for (int currVer = 0; currVer < vertices.Length - 4; currVer += 4)
			{
				vertices[currVer] = vertices[currVer + 4];
				vertices[currVer + 1] = vertices[currVer + 4 + 1];
				vertices[currVer + 2] = vertices[currVer + 4 + 2];
				vertices[currVer + 3] = vertices[currVer + 4 + 3];

			}

			//Copy and paste 4 verteces to beginning
			int j = 0;
			for (int i = vertices.Length - 4; i < vertices.Length; i++)
			{
				vertices[i] = newVerteces[j];
				j++;
			}

			//Recalculate triangles

			triangles = CalculateTriangles(tubeAmount);
		}

		private void UpdateMesh()
		{
			//Debug.Log("t/rayForm: Mesh updated.");
			mesh.Clear();

			mesh.vertices = vertices;
			mesh.triangles = triangles;

			mesh.RecalculateNormals();
		}
	}

    public void Stop()// change in future
    {
		Object.Destroy(gameObject);
    }

}
