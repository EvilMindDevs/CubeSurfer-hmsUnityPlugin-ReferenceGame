using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;

public class TableUIExample : MonoBehaviour
    {
        public TableUI table;

    public void OnChangeTextValue(string text)
        {
            int r =0;
            int c =0 ;
            string value = text;

            if (r < TableUI.MIN_ROWS - 1 || r >= table.Rows)
            {
                Debug.Log("The row number is not in range");
                return;
            }

            if (c < TableUI.MIN_COL - 1 || c >= table.Columns)
            {
                Debug.Log("The column number is not in range");
                return;
            }

            table.GetCell(r,c).text = value;



        }

        public void OnAddNewRowClick()
        {
            table.Rows++;
        }

        public void OnAddNewColumnClick()
        {
            table.Columns++;
        }

        public void OnRemoveLastColumn()
        {
            table.Columns--;
        }

        public void OnRemoveLastRow()
        {
            table.Rows--;
        }


    }

