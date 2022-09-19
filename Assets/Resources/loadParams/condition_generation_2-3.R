##### Randomizing conditions for experiments 2 and 3 #####
#
# This script creates the randomized condition orders for 
# the task allocation experiments
#
# Created by: M. Chiovaro (@mchiovaro)
# Last updated: 

##### Set up #####

# set working directory
setwd("./Assets/Resources/loadParams/")

# set seed for reproducibility
set.seed(2022)

# create a new data frame with condition columns
df <- data.frame(matrix(ncol = 17, nrow = 90))

# provide column names
colnames(df) <- c('group_number', 'com_condition',
                  'td_1', 'td_2', 'td_3', 'td_4', 
                  'td_5', 'td_6', 
                  'ie_1', 'ie_2', 'ie_3', 'ie_4', 
                  'ie_5', 'ie_6', 
                  'group_size', 'practice', 'experiment_number')

##### Experiment number #####

df$experiment_number[1:45] <- 2
df$experiment_number[46:90] <- 3
 
##### Task demand conditions #####

# for the practice round
shuffled_practice <- sample(1:3, size = nrow(df), replace = TRUE)
df$practice <- shuffled_practice

# for all rows - create the random task condition
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

# for the practice round, let both switch so they can both practice
df$practice_ie <- 3

# for all rows, create the random ie condition
for (i in 1:nrow(df)){
  
  # create a random sequence of 1 through 6 
  shuffled_ie <- sample(1:6, size = 6, replace = FALSE)
  
  # turn it into a data frame
  sample_ie <- t(as.data.frame(shuffled_ie))
  
  # fill it into a row
  df[i, c(9:14)] <- sample_ie
  
  # for each column in that row
  for (j in 9:14){
    
    # if the value is greater than 3
    if(df[i, j] > 3){
      
      # subtract 3 (making 4 -> 1, 5 -> 2, 6 -> 3)
      df[i, j] <- df[i, j] - 3 
      
    }
    
  }
  
}

##### Communication condition #####

# experiment 2
df$com_condition[1:15] <- 1
df$com_condition[16:30] <- 2
df$com_condition[31:45] <- 3

# experiment 3
df$com_condition[46:60] <- 1
df$com_condition[61:75] <- 2
df$com_condition[76:90] <- 3

##### Group size #####

# experiments 1 and 2
df$group_size[1:45] <- 2
df$group_size[46:90] <- 4

##### Shuffling rows #####

# shuffling experiment 2 rows
shuffled_data_2 = df[sample(1:45), ]

# shuffling experiment 3 rows
shuffled_data_3 = df[sample(46:90), ]

##### Bind data #####
shuffled_data_all <- rbind(shuffled_data_2, 
                            shuffled_data_3)

##### Group number #####
shuffled_data_all$group_number <- seq.int(nrow(shuffled_data_all))

# write shuffled data to file
write.table(x = shuffled_data_all,
            file='./conditions_2-3.csv',
            sep=",",
            col.names=TRUE,
            row.names=FALSE)
