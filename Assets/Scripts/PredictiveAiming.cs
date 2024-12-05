using System;
using UnityEngine;


public static class PredictiveAiming
{

    /// <summary>
    /// Calculates the firing direction needed to intercept a moving target, considering gravity.
    /// </summary>
    /// <param name="shooterPosition">The position of the shooter.</param>
    /// <param name="targetPosition">The initial position of the target.</param>
    /// <param name="targetVelocity">The constant velocity of the target.</param>
    /// <param name="projectileSpeed">The speed of the projectile.</param>
    /// <param name="gravity">The gravity vector (e.g., new Vector3(0, -9.81f, 0)).</param>
    /// <param name="firingDirection">The calculated firing direction as a unit vector.</param>
    /// <returns>True if a valid firing solution exists; otherwise, false.</returns>
    public static bool CalculateFiringSolutionWithGravity(
        Vector3 shooterPosition,
        Vector3 targetPosition,
        Vector3 targetVelocity,
        float projectileSpeed,
        Vector3 gravity,
        out Vector3 firingDirection)
    {
        //Debug.Log("Parameters: \nshooterPosition: " + shooterPosition + "\ntargetPosition: " + targetPosition + "\ntargetVelocity: " + targetVelocity + "\nprojectileSpeed: " + projectileSpeed + "\ngravity: " + gravity + "\n");

        // Initialize output
        firingDirection = Vector3.zero;

        // Relative position vector from shooter to target
        Vector3 D = targetPosition - shooterPosition;

        // Coefficients for the quartic equation: a t^4 + b t^3 + c t^2 + d t + e = 0
        float a = 0.25f * Vector3.Dot(gravity, gravity);
        float b = -Vector3.Dot(targetVelocity, gravity);
        float c = Vector3.Dot(targetVelocity, targetVelocity) - projectileSpeed * projectileSpeed - Vector3.Dot(D, gravity);
        float d = 2f * Vector3.Dot(D, targetVelocity);
        float e = Vector3.Dot(D, D);

        // Define the quartic equation as a function f(t) = a t^4 + b t^3 + c t^2 + d t + e
        System.Func<float, float> f = (t) => a * Mathf.Pow(t, 4) + b * Mathf.Pow(t, 3) + c * Mathf.Pow(t, 2) + d * t + e;

        // Define the derivative of the quartic equation f'(t)
        System.Func<float, float> df = (t) => 4f * a * Mathf.Pow(t, 3) + 3f * b * Mathf.Pow(t, 2) + 2f * c * t + d;

        // Debug.Log("quartic is: a=" + a + ", b=" + b + ", c=" + c + ", d=" + d + ", e=" + e);

        // Initial guess for t
        float t0 = 0.1f;

        // Tolerance and maximum iterations
        float tolerance = 1e-4f;
        int maxIterations = 1000;

        // Perform Newton-Raphson to find t
        float t;

        try
        {
            t = FindZero(f, df, t0, tolerance, maxIterations);
        }
        catch (global::System.InvalidOperationException)
        {
            return false;
        }

        // Calculate the firing direction using the found intercept time t
        Vector3 numerator = D + targetVelocity * t - 0.5f * gravity * t * t;
        firingDirection = numerator / (projectileSpeed * t);

        // Normalize the firing direction
        firingDirection.Normalize();

        //Debug.Log("t: " + t + "\nimpact: " + (shooterPosition + t * targetVelocity));

        return true;
    }

    // Newton's method to find the zero of a function
    private static float FindZero(System.Func<float, float> f, System.Func<float, float> df, float initialGuess, float tolerance = 1e-6f, int maxIterations = 1000)
    {
        float x = initialGuess;
        for (int i = 0; i < maxIterations; i++)
        {
            float fx = f(x);
            float dfx = df(x);
            // Debug.Log("x=" + x + ", fx=" + fx + ", dfx=" + dfx);

            // If derivative is too close to zero, method fails (to avoid division by zero)
            if (Math.Abs(dfx) < tolerance)
            {
                throw new InvalidOperationException("Derivative is too small. Newton's method fails.");
            }

            // Calculate next value
            float nextX = x - fx / dfx;

            // Check if the result is within tolerance
            if (Math.Abs(nextX - x) < tolerance)
            {
                return nextX; // Root found
            }

            x = nextX;
        }

        throw new InvalidOperationException("Maximum iterations reached. Root not found.");
    }


    // ChatGPT 4o implementation.
    /// <summary>
    /// Calculates the firing direction needed to intercept a moving target.
    /// </summary>
    /// <param name="shooterPosition">The position of the shooter.</param>
    /// <param name="targetPosition">The initial position of the target.</param>
    /// <param name="targetVelocity">The constant velocity of the target.</param>
    /// <param name="projectileSpeed">The speed of the projectile.</param>
    /// <param name="firingDirection">The calculated firing direction as a unit vector.</param>
    /// <returns>True if a valid firing solution exists; otherwise, false.</returns>
    public static bool CalculateFiringSolution(
        Vector3 shooterPosition,
        Vector3 targetPosition,
        Vector3 targetVelocity,
        float projectileSpeed,
        out Vector3 firingDirection)
    {

        // Debug.Log("Parameters: \nShooter position: " + shooterPosition + "\nTarget position: " + targetPosition + "\nTarget velocity: " + targetVelocity + "\nProjectile speed: " + projectileSpeed + "\n");

        // Initialize output
        firingDirection = Vector3.zero;

        // Relative position vector from shooter to target
        Vector3 D = targetPosition - shooterPosition;

        // Coefficients for the quadratic equation: At^2 + Bt + C = 0
        float A = Vector3.Dot(targetVelocity, targetVelocity) - projectileSpeed * projectileSpeed;
        float B = 2f * Vector3.Dot(D, targetVelocity);
        float C = Vector3.Dot(D, D);

        // If A is approximately zero, handle it as a linear equation to avoid division by zero
        const float EPSILON = 1e-6f;
        if (Mathf.Abs(A) < EPSILON)
        {
            // Debug.Log("Projectile and target have same velocity.");
            if (Mathf.Abs(B) < EPSILON)
            {
                // Debug.Log("Target moves perpendicularly to the vector between turret and target.");
                // Both A and B are zero
                // If C is also zero, shooter and target are at the same position
                if (C < EPSILON)
                {
                    // Debug.Log("Turret and target are in the same place.");
                    // Any direction is valid; default to forward
                    firingDirection = Vector3.forward;
                    return true;
                }
                // No solution exists
                //  Debug.Log("Both A and B are zero; projectile can't catch up to target.");
                return false;
            }

            // Linear solution: s = -C / B
            float s = -C / B;
            if (s < 0)
            {
                // Time cannot be negative; no solution
                // Debug.Log("Linear solution failed; the projectile can't catch up.");
                return false;
            }

            // Calculate firing direction
            firingDirection = (D + targetVelocity * s) / (projectileSpeed * s);
            firingDirection.Normalize();
            // Debug.Log("Success.");
            return true;
        }

        // Calculate discriminant
        // Debug.Log("A: " + A + "\nB: " + B + "\nC: " + C);

        float discriminant = B * B - 4f * A * C;
        // Debug.Log("discriminant: " + discriminant);


        if (discriminant < 0f)
        {
            // Debug.Log("The discriminant is negative. The projectile won't catch up to the target.");
            // No real solutions; target cannot be intercepted
            return false;
        }

        // Calculate the square root of the discriminant
        float sqrtDiscriminant = Mathf.Sqrt(discriminant);

        // Find the two possible solutions for t
        float t1 = (-B - sqrtDiscriminant) / (2f * A);
        float t2 = (-B + sqrtDiscriminant) / (2f * A);

        // Select the smallest positive t
        float t = Mathf.Min(t1, t2);
        if (t < 0f)
        {
            t = Mathf.Max(t1, t2);
            if (t < 0f)
            {
                // Both solutions are negative; no valid interception time
                // Debug.Log("Both solutions are negative!");
                return false;
            }
        }

        // Calculate the firing direction
        Vector3 aimPoint = D + targetVelocity * t;
        if (aimPoint.magnitude < EPSILON)
        {
            // Aim point is too close to determine direction
            // Debug.Log("Aiming too close! ");
            return false;
        }

        firingDirection = aimPoint / (projectileSpeed * t);
        firingDirection.Normalize();

        return true;
    }
}