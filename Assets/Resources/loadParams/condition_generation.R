##### Randomizing conditions #####
#
# This script creates the randomized condition orders for 
# the task allocation experiments
#
# Created by: M. Chiovaro (@mchiovaro)
# Last updated: 01_20_2022

##### Set up #####

# set seed for reproducibility
set.seed(2022)

# create a new data frame with condition columns
df <- data.frame(matrix(ncol = 11, nrow = 315))

# provide column names
colnames(df) <- c('group_number', 'ie_condition', 
                  'td_1', 'td_1', 'td_1', 'td_1', 
                  'td_1', 'td_1', 'com_condition', 
                  'group_size', 'experiment_number')

##### Experiment number #####

df$experiment_number[1:45] <- 1
df$experiment_number[46:135] <- 2
df$experiment_number[136:nrow(df)] <- 3
 
##### Task demand conditions #####

# for all rows
for (i in 1:nrow(df)){
    
    # create a random sequence of 1 through 6 
    shuffled <- sample(1:6, size = 6, replace = FALSE)
    
    # turn it into a data frame
    sample <- t(as.data.frame(shuffled))
    
    # fill it into a row
    df[i, c(3:8)] <- sample
    
    # for each column in that row
    for (j in 3:8){
      
      # if the value is greater than 3
      if(df[i, j] > 3){
        
        # subtract 3 (making 4 -> 1, 5 -> 2, 6 -> 3)
        df[i, j] <- df[i, j] - 3 
        
      }
      
    }
    
}

##### Individual effectivities condition #####

# experiment 1 (split by 15)
df$ie_condition[1:15] <- 1
df$ie_condition[16:30] <- 2
df$ie_condition[31:45] <- 3

# experiment 2 and 3 will only have two conditions, and is based off exp 1 
# (two conditions picked based off of least and most task switching)

# experiment 2 (split by 45)
# df$ie_condition[46:90] <- ?
# df$ie_condition[91:135] <- ?

# experiment 3 (split by 90)
# df$ie_condition[136:225] <- ?
# df$ie_condition[226:315] <- ?

##### Communication condition #####

# experiment 1
df$com_condition[1:45] <- 1

# experiment 2 - ie = 1
df$com_condition[46:60] <- 1
df$com_condition[61:75] <- 2
df$com_condition[76:90] <- 3

# experiment 2 - ie = 2
df$com_condition[91:105] <- 1
df$com_condition[106:120] <- 2
df$com_condition[121:135] <- 3

# experiment 3 - ie = 1
df$com_condition[136:165] <- 1
df$com_condition[166:195] <- 2
df$com_condition[196:225] <- 3

# experiment 3 - ie = 2
df$com_condition[226:255] <- 1
df$com_condition[256:285] <- 2
df$com_condition[286:315] <- 3

##### Group size #####

# experiments 1 and 2
df$group_size[1:135] <- 1

# experiment 3 condition 1 - ie 1
df$group_size[136:150] <- 1
df$group_size[166:180] <- 1
df$group_size[196:210] <- 1

# experiment 3 condition 1 - ie 2
df$group_size[226:240] <- 1
df$group_size[256:270] <- 1
df$group_size[286:300] <- 1

# experiment 3 condition 2 - ie 1
df$group_size[151:165] <- 2
df$group_size[181:195] <- 2
df$group_size[211:225] <- 2

# experiment 3 condition 2 - ie 2
df$group_size[241:255] <- 2
df$group_size[271:285] <- 2
df$group_size[301:315] <- 2

##### Shuffling rows #####

# shuffling experiment 1 rows
shuffled_data_1 = df[sample(1:45), ]

# shuffling experiment 2 rows
shuffled_data_2 = df[sample(46:135), ]

# shuffling experiment 3 rows
shuffled_data_3 = df[sample(136:315), ]

##### Save data #####
# shuffled_data_all <- rbind(shuffled_data_1, 
#                            shuffled_data_2,
#                            shuffled_data_3)

# write shuffled data to file
write.table(x = shuffled_data_all,
            file='./conditions.csv',
            sep=",",
            col.names=TRUE,
            row.names=FALSE)
