using Godot;

public class PlayerController : KinematicBody
{
	[Export] public int Speed = 10;
	[Export] public float gravity = 0.7f;

	private Vector3 velocity = new Vector3();

	private Label NameLabel { get; set; }

	[Puppet]
	public Vector3 PuppetPosition { get; set; }
	[Puppet]
	public Vector3 PuppetVelocity { get; set; }

	public void GetInput()
	{
		float ySpeed = velocity.y;
		velocity = new Vector3();

		if (Input.IsActionPressed("move_right"))
			velocity.x += 1;

		if (Input.IsActionPressed("move_left"))
			velocity.x -= 1;

		if (Input.IsActionPressed("move_forward"))
			velocity.z += 1;

		if (Input.IsActionPressed("move_back"))
			velocity.z -= 1;

		velocity = velocity.Normalized() * Speed;
		
		velocity.y = ySpeed - gravity;

		Rset(nameof(PuppetPosition), Transform.origin);
		Rset(nameof(PuppetVelocity), velocity);
	}

	public override void _Ready() {
		if (IsNetworkMaster())
		{
			MainCamera.GetInstance().SetFollowTarget(this);
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (IsNetworkMaster())
		{
			GetInput();
		}
		else
		{
			Transform t = Transform;
			t.origin = PuppetPosition;
			Transform = t;
			
			velocity = PuppetVelocity;
		}
		
		velocity = MoveAndSlide(velocity);

		if (!IsNetworkMaster())
		{
			PuppetPosition = Transform.origin;
		}
	}

	public void SetPlayerName(string name)
	{
		NameLabel = (Label)GetNode("LabelOrigin/Viewport/Label");

		PuppetPosition = Transform.origin;
		PuppetVelocity = velocity;

		NameLabel.Text = name;
	}
}
