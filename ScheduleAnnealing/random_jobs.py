#!/usr/bin/env python3

# skrypt nie działa dobrze bo czasami generuje cykle zależności

import random

duration_range = (1, 100)
dependence_rate = 0.2
amount = 100

jobs = [random.randint(duration_range[0], duration_range[1]) for _ in range(amount)]
dependencies = []
for _ in range(int(amount * dependence_rate)):
    dependent = -1
    required = -1
    while dependent == required:
        dependent = random.randint(0, amount-1)
        required = random.randint(0, amount-1)

    dependencies.append((dependent, required))

with open("generatedjobs.txt", "w") as file:
    for job in jobs[:-1]:
        file.write(str(job) + " ")
    file.write(f'{str(jobs[-1])}\n')

    for dependent, required in dependencies:
        file.write(str(dependent) + ' ' + str(required) + '\n')
