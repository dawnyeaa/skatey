using UnityEngine;

public abstract class SkateboardBaseState : State {
  protected readonly SkateboardStateMachine stateMachine;
  protected float TurnSpeed = 0.0f;

  protected SkateboardBaseState(SkateboardStateMachine stateMachine) {
    this.stateMachine = stateMachine;
  }

  protected void BeginPush() {
    if ((stateMachine.Decelerating && stateMachine.DecelTime >= stateMachine.PushCooldown) || (!stateMachine.Pushing && !stateMachine.Decelerating)) {
      stateMachine.PushTime = 0;
      stateMachine.Pushing = true;
      stateMachine.Decelerating = false;
    }
  }

  protected void BeginSlowdown() {
    stateMachine.CoastSpeed = stateMachine.CurrentSpeed;
    stateMachine.DecelTime = 0;
    stateMachine.Decelerating = true;
    stateMachine.Pushing = false;
  }

  protected void Accelerate() {
    // acceleration stuff first
    // theres a pushing duration, it increases the speed by a curve over its duration - to emulate pushing
    if (stateMachine.Pushing) {
      stateMachine.PushTime += Time.deltaTime;
      float pushRemapped = stateMachine.PushAccelCurve.Evaluate(stateMachine.PushTime/stateMachine.PushDuration);
      float velocityPushFactor = stateMachine.PushSpeedFalloff.Evaluate(stateMachine.CurrentSpeed/stateMachine.MaxSpeed);
      float newSpeed = stateMachine.CurrentSpeed+(stateMachine.PushAccel*Time.deltaTime*pushRemapped*velocityPushFactor);
      // float cappedSpeed = Mathf.Min(newSpeed, stateMachine.MaxSpeed);
      // float pushExcess = (newSpeed-cappedSpeed)*stateMachine.ExcessPushStrength;
      stateMachine.CurrentSpeed = newSpeed;
      if (stateMachine.PushTime >= stateMachine.PushDuration) {
        BeginSlowdown();
      }
    }
    // then decelerate
    // if we're slowing down
    else if (stateMachine.Decelerating) {
      stateMachine.DecelTime += Time.deltaTime;
      stateMachine.CurrentSpeed = (stateMachine.Mass*stateMachine.CoastSpeed)/((stateMachine.Drag*stateMachine.DragMultiplier*stateMachine.CoastSpeed*stateMachine.DecelTime)+stateMachine.Mass);
      if (stateMachine.CurrentSpeed <= 0) {
        stateMachine.CurrentSpeed = 0;
        stateMachine.Decelerating = false;
      }
    }
  }

  protected void CalculateTurn() {
    if (stateMachine.CurrentSpeed > 0) {
      stateMachine.Turning = Mathf.SmoothDamp(stateMachine.Turning, stateMachine.Input.turn, ref TurnSpeed, stateMachine.TurnSpeedDamping);
      float deltaPos = Time.deltaTime*stateMachine.CurrentSpeed;
      float rad = stateMachine.TruckSpacing/Mathf.Sin(Mathf.Deg2Rad*stateMachine.Turning*stateMachine.MaxTruckTurnDeg);
      float circum = ThisIsJustTau.TAU*rad;
      stateMachine.Facing += deltaPos/circum;
      stateMachine.DragMultiplier = 1.0f + Mathf.Abs(stateMachine.Turning)*stateMachine.TurnSlowdown;
    }
  }

  protected void ApplyGravity() {
    // gravity stuff here????
  }

  protected void Move() {
    stateMachine.transform.rotation = Quaternion.AngleAxis(stateMachine.Facing*360f, stateMachine.transform.up);
    stateMachine.transform.position += stateMachine.CurrentSpeed * Time.deltaTime * stateMachine.transform.forward;
  }

  protected void SpinWheels() {
    float circum = ThisIsJustTau.TAU*stateMachine.WheelRadius;
    stateMachine.Wheels.AddRotation(((Time.deltaTime*stateMachine.CurrentSpeed)/circum)*360f);
  }
}