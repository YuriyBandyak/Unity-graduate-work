using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that keep all number for balancing a magic system
/// </summary>
public static class MagicBalance
{
    // FORMS
    public static class Directed
    {
        public static float size = 1f;
        public static float speed = 4f;
        public static float power = 40f;
        public static float speedOfDisappearing = 0.003f;
    }

    //wall
    public static class Wall
    {
        public static float period = 10;
        //public static float time
        public static float power = 10;
    }

    //cone
    public static class Cone
    {
        public static float distance = 1.5f;
        public static float particlesAmount = 0;// not implemented
        public static float power = 10;
    }

    //ring
    public static class Ring
    {
        public static float radius = 1.5f;
        public static float minParticleAmount = 20;
        public static float maxParticleAmount = 100;
        public static float interpolateVal = 0.9f; //Value used to interpolate between min and max
        public static float power = 10;
    }

    //ray
    public static class Ray
    {
        public static int maxDistance = 10;
        public static float speed = 0.2f;

        public static float rotationSpeed = 666; // use it in playerstatsandfuncrion or playerMovement to dicrease speed of rotating with casting ray spell
        public static float power = 10;
    }

    //selfCast
    public static class SelfCast
    {
        public static float duration = 5;
        public static float power = 1; // ??
    }


}
