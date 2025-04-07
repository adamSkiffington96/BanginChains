
# Bangin Chains

Disc golf simulation with accurate lift mechanics, and the ability to tilt your disc hyzer/anhyzer before a throw


## Screenshots

![App Screenshot](https://i.imgur.com/5MJ70PU.jpeg)


## Features

- Lift mechanics
- Throw disc at different angles
- Disc curve
- Follow cam
- Return disc
  

## Snippets

</details>

<details>
<summary><code>CalculateLift</code></summary>

```
private void CalcLift()
{
    // Define a drag force
    //Vector3 dragForce = (-drag * rb.velocity) * Time.deltaTime;
    // Drag now calculated in rigidbody

    // Define normal force as the disc normal * our velocity
    Vector3 nForce = liftModifier * rb.velocity.sqrMagnitude * Vector3.Project(-Physics.gravity, DiscHandle.up.normalized).normalized;

    // Clamp lift mag to gravity's magnitude 
    if (nForce.magnitude > Physics.gravity.magnitude)
        nForce = Vector3.Project(-Physics.gravity, DiscHandle.up.normalized);

    Vector3 liftForce = Time.deltaTime * nForce;

    // Curve mechanics
    Vector3 sideDir = Vector3.Cross(DiscHandle.up, rb.velocity).normalized;
    Vector3 curve = (sideDir * curveAmount * curvePower) * Time.deltaTime;

    // Apply the lift force and curve mechanics
    rb.velocity += (liftForce + curve);

    //DiscHandle.position += (_velocity) * Time.deltaTime;
    // Position now calculated in rigidbody

    _debugText.text = "Lift: " + liftForce.magnitude + "Gravity: " + Physics.gravity.magnitude + "\nVelocity: " + rb.velocity.magnitude;
}
```
</details>

</details>
