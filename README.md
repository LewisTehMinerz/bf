# bf
debugging brainf**k interpreter, because this is obviously needed

# commands
| command | meaning                                               |
|---------|-------------------------------------------------------|
| >       | increment memory pointer                              |
| <       | decrement memory pointer                              |
| +       | add to memory cell                                    |
| -       | subtract from memory cell                             |
| .       | output cell as byte                                   |
| ,       | input byte and write to memory cell                   |
| [       | step into code if memory cell is 0                    |
| ]       | jump to start of loop if memory cell is not 0         |
| $       | trigger breakpoint                                    |
| #       | toggle step-through mode (may conflict with comments) |
