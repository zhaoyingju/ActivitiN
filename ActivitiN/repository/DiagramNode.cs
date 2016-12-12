namespace org.activiti.engine.repository
{
	public class DiagramNode : DiagramElement
	{

	  private double? x = null;
	  private double? y = null;
	  private double? width = null;
	  private double? height = null;

	  public DiagramNode() : base()
	  {
	  }

	  public DiagramNode(string id) : base(id)
	  {
	  }

	  public DiagramNode(string id, double? x, double? y, double? width, double? height) : base(id)
	  {
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	  }

	  public virtual double? X
	  {
		  get
		  {
			return x;
		  }
		  set
		  {
			this.x = value;
		  }
	  }


	  public virtual double? Y
	  {
		  get
		  {
			return y;
		  }
		  set
		  {
			this.y = value;
		  }
	  }


	  public virtual double? Width
	  {
		  get
		  {
			return width;
		  }
		  set
		  {
			this.width = value;
		  }
	  }


	  public virtual double? Height
	  {
		  get
		  {
			return height;
		  }
		  set
		  {
			this.height = value;
		  }
	  }


	  public override string ToString()
	  {
		return base.ToString() + ", x=" + X + ", y=" + Y + ", width=" + Width + ", height=" + Height;
	  }

	  public override bool Node
	  {
		  get
		  {
			return true;
		  }
	  }

	  public override bool Edge
	  {
		  get
		  {
			return false;
		  }
	  }

	}

}