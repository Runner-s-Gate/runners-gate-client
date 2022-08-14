using Godot;
using System;

public class MainCamera : Camera
{
	public static MainCamera _instance;
	
	[Export] public Vector3 Offset = new Vector3(0f, 6f, 5f);
	
	private Spatial followTarget;
	
	public static MainCamera GetInstance() {
		return _instance;
	}
	
	public override void _Ready()
	{
		if(_instance == null) {
			_instance = this;
		}
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(this.followTarget != null) {
			Transform t = this.Transform;
			t.origin = this.followTarget.Transform.origin + Offset;
			this.Transform = t;
		}
	}
	
	public void SetFollowTarget(Spatial followTarget) {
		this.followTarget = followTarget;
	}

}
