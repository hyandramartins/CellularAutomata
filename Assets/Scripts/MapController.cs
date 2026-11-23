using UnityEngine;

public class MapController
{
    private bool conectivityValidated;
    private bool opennessValidated;
    private bool complexyValidated;

    public bool ValidateMap(double connectivityScore, double opennessScore, double complexyScore)
    {
        if (connectivityScore >= 0.25)
        {
            Debug.Log("Validated connectivityScore");
            conectivityValidated = true;
        }

        if (opennessScore >= 0.35 && opennessScore <= 0.65)
        {
            Debug.Log("Validated opennessScore");
            opennessValidated = true;

        }

        if (complexyScore >= 0.7)
        {
            Debug.Log("Validated complexyScore");
            complexyValidated = true;
        }

        if(conectivityValidated && opennessValidated && complexyValidated)
        {
            return true;
        }

        return false;
    }
}
